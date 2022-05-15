using Assets.Scripts.Machine;
using AxiOMADataTest;
using System.Collections;
using System.Threading;
using UnityEngine;

public class Client : MonoBehaviour
{
    private Form1 form;
    private Thread thread;

	public bool isRun;
	public bool isChangeTool;
	public bool permissionChange;
	public bool start;

	public float x;
	public float y;
	public float z;
	public PlcHandler plcHandler;

	public void RunClient()
	{
		form = Program.GetForm();
		plcHandler = new PlcHandler(form);
		isRun = true;
	}

	private void Start()
	{
		thread = new Thread(() => Initialization());
		thread.Start();
	}

	private void Update()
    {
		if (isRun && !isChangeTool)
		{
			if (form.x != 0)
				x = (float)form.x / 1000;

				y = -0.610f - ((float)form.z / 1000 * -1);

			if (form.y != 0)
				z = (float)form.y / 1000;
		}
	}

	public void ChangeTool()
	{
		StartCoroutine(ChangePositionForChangeTool());
	}

	private void Initialization() => Program.CreateWindow();

	public IEnumerator ChangePositionForChangeTool()
	{
		start = true;
		if(!isChangeTool)
		{
			isChangeTool = true;
			while (y <= 0)
			{
				y += 0.002f;
				yield return null;
			}
			permissionChange = true;
			StopCoroutine(ChangePositionForChangeTool());
		}
		else
		{
			permissionChange = false;
			while (y > -0.610f)
			{
				y -= 0.002f;
				yield return null;
			}
			isChangeTool = false;
			StopCoroutine(ChangePositionForChangeTool());
		}
	}
}
