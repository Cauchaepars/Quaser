using AxiOMADataTest;

namespace Assets.Scripts.Machine
{
    public class PlcHandler
    {
        public enum HandInputStates { C1, _90_tool, Lock_tool, Hand_D, _180an, Hand_U, _90_Def, None}
        public enum HandOutputStates {_90_tool, Lock_tool, Hand_down, Hand_up, _180an, _90_default, None }
        public enum ShopToolStates { CwRotation, CcwRotation, C1, None }
        public enum ShopToolInputStates { LRR, RRR, None }

        public HandInputStates handInputState;
        public HandOutputStates handOutupState;
        public ShopToolStates shopToolState;
        public ShopToolInputStates shopToolInputState;

        private int numberBitHand;
        private int numberBitShopTool;

        public byte numberCurrentTool = 0;
        public int numberTool = 100;

        private readonly Form1 form;

        public PlcHandler(Form1 form)
        {
            handInputState = HandInputStates.None;
            handOutupState = HandOutputStates.None;
            shopToolState = ShopToolStates.None;
            this.form = form;
		}

        public void WritePlcRotate()
        {
            for (int i = 0; i < form.plcRotateInput.Length; i++)
                form.plcRotateInput[i] = false;

            switch (shopToolInputState)
            {
                case ShopToolInputStates.RRR:
                    form.plcRotateInput[0] = true;
                    break;

                case ShopToolInputStates.LRR:
                    form.plcRotateInput[1] = true;
                    break;

				case ShopToolInputStates.None:
					break;
			}
        }

        public void ReadPlcRotate()
        {
            numberBitShopTool = 3;
            numberCurrentTool = form.currentluNumberTool;
            numberTool = form.numberTool;

            for (int i = 0; i < form.plcRotateOutput.Length; i++)
            {
                if(form.plcRotateOutput[i] == true)
                    numberBitShopTool = i;
            }

            switch (numberBitShopTool)
            {
                case 0:
                    shopToolState = ShopToolStates.CwRotation;
                    break;

                case 1:
                    shopToolState = ShopToolStates.CcwRotation;
                    break;

                case 2:
                    shopToolState = ShopToolStates.C1;
                    break;

                case 3:
                    shopToolState = ShopToolStates.None;
                    break;
            }
        }

        public void ReadPlc()
        {
            numberBitHand = 6;

            for (int i = 0; i < form.outputValue.Length; i++)
            {
                if (form.outputValue[i] == true)
                {
                    numberBitHand = i;
                }
            }

            switch (numberBitHand)
            {
                case 0:
                    handOutupState = HandOutputStates._90_tool;
                    break;

                case 1:
                    handOutupState = HandOutputStates.Lock_tool;
                    break;

                case 2:
                    handOutupState = HandOutputStates.Hand_down;
                    break;

                case 3:
                    handOutupState = HandOutputStates.Hand_up;
                    break;

                case 4:
                    handOutupState = HandOutputStates._180an;
                    break;

                case 5:
                    handOutupState = HandOutputStates._90_default;
                    break;

                case 6:
                    handOutupState = HandOutputStates.None;
                    break;
            }
        }

        //Двигатель
        public void Impulse(bool value)
        {
            form.impuls = value;
        }

        public void WritePlc()
        {
			for (int i = 0; i < form.inputValue.Length; i++)
                form.inputValue[i] = false;

			switch (handInputState)
            {
                case HandInputStates.C1:
                    form.inputValue[0] = true;
                    break;

                case HandInputStates._90_tool:
                    form.inputValue[1] = true;
                    break;

                case HandInputStates.Lock_tool:
                    form.inputValue[2] = true;
                    break;

                case HandInputStates.Hand_D:
                    form.inputValue[3] = true;
                    break;

                case HandInputStates._180an:
                    form.inputValue[4] = true;
                    break;

                case HandInputStates.Hand_U:
                    form.inputValue[5] = true;
                    break;

                case HandInputStates._90_Def:
                    form.inputValue[6] = true;
					break;

                case HandInputStates.None:
                {
                    for (int i = 0; i < form.inputValue.Length; i++)
                            form.inputValue[i] = false;
                        break;
                }
            }
        }
    }
}

