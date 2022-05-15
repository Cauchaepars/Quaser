using UnityEngine;
using Assets.Scripts.Machine;
using System.Collections;

public class ShopTool : MonoBehaviour
{
	//Параметры для вращения магазина
	private readonly float angle = 12f;
	private readonly float speed = 0.1f;

	//Номер сменяемого инструмента
	public static int number = 0;
	//Тееущий угол поворота
	private float _x = 0;
	//Состояние, которое необходимо для полного поворота
	private PlcHandler.ShopToolStates temporaryState;
	//Для корутины
	private bool start;

	private Client client;

	private void Start()
	{
		client = GameObject.Find("Main Camera").GetComponent<Client>();
	}

	private void Update()
	{
		if (client.isRun)
		{
			client.plcHandler.ReadPlcRotate();

			if ((client.plcHandler.shopToolState == PlcHandler.ShopToolStates.CwRotation
				|| client.plcHandler.shopToolState == PlcHandler.ShopToolStates.CcwRotation)
				&& !start)
			{
				if(!client.start)
					client.ChangeTool();

				if(client.permissionChange)
				{
					temporaryState = client.plcHandler.shopToolState;
					StartCoroutine(Rotate());
				}
			}
		}
	}

	private IEnumerator Rotate()
	{
		start = true;

		if (temporaryState == PlcHandler.ShopToolStates.CwRotation)
		{
			float x = 0;
			while (x <= angle)
			{
				x += speed;
				_x += speed;
				transform.localRotation = Quaternion.Euler(_x, 0, 0);

				yield return null;
			}

			client.plcHandler.shopToolInputState = PlcHandler.ShopToolInputStates.RRR;
			client.plcHandler.WritePlcRotate();

			yield return new WaitForSecondsRealtime(0.2f);

			client.plcHandler.shopToolInputState = PlcHandler.ShopToolInputStates.None;
			client.plcHandler.WritePlcRotate();
		}

		if (temporaryState == PlcHandler.ShopToolStates.CcwRotation)
		{
			float x = 0;
			while (x <= angle)
			{
				x += speed;
				_x -= speed;
				transform.localRotation = Quaternion.Euler(_x, 0, 0);

				yield return null;
			}

			client.plcHandler.shopToolInputState = PlcHandler.ShopToolInputStates.LRR;
			client.plcHandler.WritePlcRotate();

			yield return new WaitForSecondsRealtime(0.2f);

			client.plcHandler.shopToolInputState = PlcHandler.ShopToolInputStates.None;
			client.plcHandler.WritePlcRotate();

		}

		StopCoroutine(Rotate());

		temporaryState = client.plcHandler.shopToolState;

		if (client.plcHandler.numberCurrentTool == client.plcHandler.numberTool)
		{
			number = client.plcHandler.numberTool;
			client.plcHandler.Impulse(true);
			client.start = false;
		}

		start = false;
	}
}
