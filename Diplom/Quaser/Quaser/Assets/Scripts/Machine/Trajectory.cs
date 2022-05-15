using UnityEngine;

public class Trajectory : MonoBehaviour
{
	[SerializeField]
	private LineRenderer line;

	Client client;

    private void Start()
    {
        client = GameObject.Find("Main Camera").GetComponent<Client>();
		line.positionCount = 0;
	}

    private void Update()
    {
        if(client.isRun)
        {
			Vector3 currentPoint = transform.position * -1;
			line.positionCount++;
			line.SetPosition(line.positionCount - 1, currentPoint);
		}
    }

    public void ShowTrajectory()
    {
		line.enabled = !line.enabled;
	}

	public void ResetTrajectory()
	{
		line.positionCount = 0;
	}
}
