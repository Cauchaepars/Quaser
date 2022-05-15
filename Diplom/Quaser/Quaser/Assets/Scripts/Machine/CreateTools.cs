using Assets.Scripts.Machine;
using AxiOMADataTest;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateTools : MonoBehaviour
{
    //������ ������������
    public static List<Tool> tools;

	//���������� ��� ��������� �������
	private const float X = 0.4665f;
	private const float Y = 0.9028f;
	private const float Z = -0.0695f;

	//������� � �������� �������
	private Vector3 currentlyPositionTool;
    private Quaternion currentlyRotationTool;

    [SerializeField]
    private GameObject spindle;

    private void Start()
    {
        tools = new List<Tool>();
        currentlyPositionTool = new Vector3(X, Y, Z);
        currentlyRotationTool = Quaternion.Euler(0, 0, 90);

        #region ���������� �������� ������������
        for (int i = 0; i < 30; i++)
        {
            transform.rotation = Quaternion.Euler(i * 12f, 0, 0);

            tools.Add(new Tool
            {
                Name = $"Change_Tool_{i}",
                Position = currentlyPositionTool,
                Rotation = currentlyRotationTool,
                ParentTransform = gameObject.transform,
                Number = i
            });
            tools[i].AddToolInShopTool();
        }
        transform.rotation = Quaternion.Euler(0, 0, 0);
        #endregion

        //�������� ����������� � ��������(�������� �����������)
        Hand.CurrentTool = new Tool
        {
            Name = $"Main_Tool",
            Position = new Vector3(0, 0, 0),
            Rotation = Quaternion.Euler(0, 0, 0),
            ParentTransform = spindle.transform,
            Number = 1000,
            Length = 100,
            Radius = 30
        };
        Hand.CurrentTool.AddToolInSpindle();
    }

    public void UpdateTools()
    {
        for (int i = 0; i < 30; i++)
        {
            tools[i].UpdateTool();
        }

        Hand.CurrentTool.UpdateToolInSpindle();
    }
}
