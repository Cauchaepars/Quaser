using AxiOMADataTest;
using UnityEngine;

namespace Assets.Scripts.Machine
{
    public class Tool : MonoBehaviour
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public float Length 
        { 
            get
            {
                return length;
            }
            set
            {
                if (value > 300f)
                {
                    length = 300f;
                }
                else if (value < 50f)
                {
                    length = 50f;
                }
                else
                {
                    length = value;
                }
            }
        }
        public float Radius 
        { 
            get
            {
                return radius;
            }
            set
            {
                if (value > 76.2f)
                {
                    radius = 76.2f;
                }
                else if (value < 5f)
                {
                    radius = 5f;
                }
                else
                {
                    radius = value;
                }
            }
        }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Transform ParentTransform { get; set; }

        public GameObject ToolObject;
        public Vector3 PositionPoint;

        private float length;
        private float radius;
        private float positionY;
        private GameObject CutterObject;

        private readonly GameObject ToolInPatronObject;
        private readonly GameObject ToolObjectLoad;

		private const float COEF_X = 0.00034f;
		private const float COEF_Z = 0.00037f;
		private const float COEF_Y = 0.000275f;

		public Tool()
        {
            ToolInPatronObject = Resources.Load<GameObject>("Patron");
            ToolObjectLoad = Resources.Load<GameObject>("Цанговый патрон");

            Length = 300;
            Radius = 100;
        }

        public void AddToolInShopTool()
        {
            for (int i = 0; i < Hand.ChangeTools.Count; i++)
            {
                if (Number == Hand.ChangeTools[i].Number)
                {
                    Destroy(Hand.PatronObjects[i]);
                    Hand.PatronObjects.RemoveAt(i);
                    Hand.ChangeTools.RemoveAt(i);
                }
            }

            Hand.PatronObjects.Add(Instantiate(ToolInPatronObject, ParentTransform));
            Hand.PatronObjects[Number].transform.SetPositionAndRotation(Position, Rotation);

            PositionPoint = Hand.PatronObjects[Number].transform.GetChild(2).position;
            ToolObject = Hand.PatronObjects[Number].transform.GetChild(1).gameObject;
            CutterObject = ToolObject.transform.GetChild(0).gameObject;
            positionY = CutterObject.transform.localPosition.y;

            Hand.ChangeTools.Add(this);
        }

        public void AddToolInSpindle()
        {
            ToolObject = Instantiate(ToolObjectLoad, ParentTransform);
            ToolObject.transform.SetPositionAndRotation(Position, Rotation);
            ToolObject.tag = "CurrentTool";
            CutterObject = ToolObject.transform.GetChild(0).gameObject;
            positionY = CutterObject.transform.localPosition.y;

            Hand.CurrentTool = this;
            ChangeSize();
            TextUpdate.Change(this);
        }

        public void ChangeSize()
        {
			CutterObject.transform.localScale = new Vector3(radius * COEF_X * 2, length * COEF_Y, radius * COEF_Z * 2);
			CutterObject.transform.localPosition = new Vector3(CutterObject.transform.localPosition.x,
				positionY - (length * COEF_Y - 0.02f),
			CutterObject.transform.localPosition.z);
        }

        public void UpdateTool()
        {
            Length = Random.Range(0f, 300f);
            Radius = Random.Range(5f, 76.2f);
            ChangeSize();
        }

        public void UpdateToolInSpindle()
        {
			//Length = Form1.toolParam_1.cutting_edge[0].lenght1;
			//Radius = Form1.toolParam_1.cutting_edge[0].radius;
			//Name = Form1.toolParam_1.ToolName;
			Length = Random.Range(0f, 300f);
            Radius = Random.Range(5f, 76.2f);
            Name = "Tool";
			ChangeSize();
            TextUpdate.Change(this);
        }
    }
}
