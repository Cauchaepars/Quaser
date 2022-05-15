using Assets.Scripts.Machine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    //Положение руки по координате Y
    private float cur_Y;

    //Запущена ли корутина
    private bool start;
    private bool startPneumatic;

    //Сдвиг для центрирования инструмента в патроне
    private const float Z = 0.0077f;
    private const float Y = -0.85f;
    private const float X = 0.0002f;

    //Направление вращения патрона
    private bool rotateToCapture = true;

    private readonly float speed = 1f;
    private bool change;
    private bool down;

    private GameObject Hand_obj;

    [SerializeField]
    private GameObject ShopToolObject;
    [SerializeField]
    private GameObject SpindleObject;

    public static Tool CurrentTool;
    public static List<Tool> ChangeTools;
    public static List<GameObject> PatronObjects;

    private Client client;

    private void Start()
	{
        PatronObjects = new List<GameObject>();
        ChangeTools = new List<Tool>();

        Hand_obj = gameObject;
        cur_Y = transform.position.y;

        client = GameObject.Find("Main Camera").GetComponent<Client>();
    }

	void Update()
	{
		if (client.isRun)
		{
			client.plcHandler.ReadPlc();
			client.plcHandler.ReadPlcRotate();

			if (!start && client.plcHandler.handOutupState != PlcHandler.HandOutputStates.None)
			{
				StartCoroutine(ChangeTool());
			}

			if (client.plcHandler.shopToolState == PlcHandler.ShopToolStates.C1 && !startPneumatic)
			{
				StartCoroutine(Rotate());
			}

			if (client.plcHandler.handOutupState == PlcHandler.HandOutputStates.None && !startPneumatic && down)
			{
				StartCoroutine(Rotate());
			}
		}
	}

	private void WritePlc()
	{
		StopCoroutine(ChangeTool());
		client.plcHandler.WritePlc();
	}

	private void ChangeParent()
	{
		//Перепривязка родителей
		ChangeTools[ShopTool.number].ToolObject.transform.parent = SpindleObject.transform;
		CurrentTool.ToolObject.transform.parent = PatronObjects[ShopTool.number].transform;

		//Меняем объекты местами
		(CurrentTool, ChangeTools[ShopTool.number]) = (ChangeTools[ShopTool.number], CurrentTool);

		CurrentTool.ToolObject.tag = "CurrentTool";
		ChangeTools[ShopTool.number].ToolObject.tag = "Tool";

		//Сдвиг к центру патрона и шпинделя
		ChangeTools[ShopTool.number].ToolObject.transform.localPosition = new Vector3(X, Y, Z);
		ChangeTools[ShopTool.number].ToolObject.transform.localRotation = Quaternion.Euler(0, 0, 0);

		CurrentTool.ToolObject.transform.localPosition = new Vector3(0, 0, 0);
		CurrentTool.ToolObject.transform.localRotation = Quaternion.Euler(0, 0, 0);

		TextUpdate.Change(CurrentTool);
		change = true;
	}

	#region Алгоритм смены инструмента
	public IEnumerator ChangeTool()
	{
		start = true;
		switch (client.plcHandler.handOutupState)
		{
			//Поворот руки на 180 градусов
			case PlcHandler.HandOutputStates._90_tool:
				{
					float y = 90;
					while (y <= 180)
					{
						y += speed;
						transform.localRotation = Quaternion.Euler(0, y, 0);

						yield return null;
					}
					client.plcHandler.handInputState = PlcHandler.HandInputStates._90_tool;
					ChangeTools[ShopTool.number].ToolObject.transform.parent = Hand_obj.transform;
					CurrentTool.ToolObject.transform.parent = Hand_obj.transform;
					change = false;

					break;
				}
			//Выдвижение руки
			case PlcHandler.HandOutputStates.Hand_down:
				{
					float y = cur_Y;
					while (y >= cur_Y - 0.074f)
					{
						y -= speed / 150;
						transform.position = new Vector3(transform.position.x, y, transform.position.z);

						yield return null;
					}
					client.plcHandler.handInputState = PlcHandler.HandInputStates.Hand_D;
					break;
				}
			//Проворот руки на 360 градусов
			case PlcHandler.HandOutputStates._180an:
				{
					float y = 180;
					while (y >= 0)
					{
						y -= speed;
						transform.localRotation = Quaternion.Euler(0, y, 0);
						yield return null;
					}
					client.plcHandler.handInputState = PlcHandler.HandInputStates._180an;
					break;
				}
			//Задвижение руки
			case PlcHandler.HandOutputStates.Hand_up:
				{
					float y = cur_Y - 0.074f;
					while (y <= cur_Y)
					{
						y += speed / 150;
						transform.position = new Vector3(transform.position.x, y, transform.position.z);

						yield return null;
					}
					client.plcHandler.handInputState = PlcHandler.HandInputStates.Hand_U;
					break;
				}
			//Проворот руки на 90 градусов
			case PlcHandler.HandOutputStates._90_default:
				{
					if (!change)
						ChangeParent();

					float y = 0;
					while (y >= -90)
					{
						y -= speed;
						transform.localRotation = Quaternion.Euler(0, y, 0);

						yield return null;
					}
					client.plcHandler.handInputState = PlcHandler.HandInputStates._90_Def;

					//client.plcHandler.Impulse(false);
					down = true;
					startPneumatic = false;
					break;
				}
		}

		WritePlc();

		yield return new WaitForSecondsRealtime(0.1f);
		start = false;
	}
	#endregion

	#region Поворот патрона
	public IEnumerator Rotate()
	{
		startPneumatic = true;

		if (rotateToCapture)
		{
			while (PatronObjects[ShopTool.number].transform.rotation.eulerAngles.z >= 1f)
			{
				PatronObjects[ShopTool.number].transform.RotateAround(CreateTools.tools[ShopTool.number].PositionPoint,
					Vector3.back,
					Mathf.Lerp(0, 90, 0.01f));

				yield return null;
			}
			client.plcHandler.handInputState = PlcHandler.HandInputStates.C1;
			rotateToCapture = false;
			client.plcHandler.Impulse(false);
		}
		else
		{
			while (PatronObjects[ShopTool.number].transform.rotation.eulerAngles.z <= 90)
			{
				PatronObjects[ShopTool.number].transform.RotateAround(CreateTools.tools[ShopTool.number].PositionPoint,
					Vector3.back,
					Mathf.Lerp(0, -90, 0.01f));

				yield return null;
			}

			client.plcHandler.handInputState = PlcHandler.HandInputStates.None;
			client.ChangeTool();

			rotateToCapture = true;
			down = false;
		}

		client.plcHandler.WritePlc();
		StopCoroutine(Rotate());
		yield return new WaitForSecondsRealtime(0.1f);
		startPneumatic = false;
	}
	#endregion
}
