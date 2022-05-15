using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField] private bool isTable;
    [SerializeField] private bool isGuide;

	private Client client;

	private void Start()
	{
		client = GameObject.Find("Main Camera").GetComponent<Client>();
	}

	void Update()
    {
		if (client.isRun)
		{
			if (gameObject.tag == "CurrentTool" && !client.permissionChange)
			{
				gameObject.transform.position = new Vector3(gameObject.transform.position.x,
					client.y,
					gameObject.transform.position.z);
			}

			if (isTable)
			{
				gameObject.transform.position = new Vector3(client.x,
					gameObject.transform.position.y,
					client.z);
			}

			if (isGuide)
			{
				gameObject.transform.position = new Vector3(gameObject.transform.position.x,
					gameObject.transform.position.y,
					client.z);
			}
		}
	}

    public void Disconnect()
    {
		client.isRun = false;
		transform.position = Vector3.zero;
	}
}