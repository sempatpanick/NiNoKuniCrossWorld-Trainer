using Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NNKCW_External.Elevate;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NNKCW_External
{

    public partial class External : Form
    {
        Thread t;
        string processName = "ProjectN-Win64-Shipping";
        Mem m = new Mem();

        private readonly String _addressHP = "ProjectN-Win64-Shipping.exe+063818E8,0,20,558";
        private readonly String _addressCoordinateX = "ProjectN-Win64-Shipping.exe+0638A5A0,DE8,38,0,30,260,290,1D0";
        private readonly String _addressCoordinateY = "ProjectN-Win64-Shipping.exe+0638A5A0,DE8,38,0,30,260,290,1D4";
        private readonly String _addressCoordinateZ = "ProjectN-Win64-Shipping.exe+0638A5A0,DE8,38,0,30,260,290,1D8";
        private readonly String _addressTimeDilation = "ProjectN-Win64-Shipping.exe+063818E8,0,20,98";
        private readonly String _addressAttackSpeed = "ProjectN-Win64-Shipping.exe+063818E8,0,20,804";
        private readonly String _addressMovementSpeed = "ProjectN-Win64-Shipping.exe+063818E8,0,20,288,18C";
        private readonly String _addressMultiJumpCount = "ProjectN-Win64-Shipping.exe+063818E8,0,20,344";
        private readonly String _addressJumpZVelocity = "ProjectN-Win64-Shipping.exe+063818E8,0,20,288,158";
        private readonly String _addressCancelAnimation = "ProjectN-Win64-Shipping.exe+063818E8,0,20,514";
        private readonly String _addressAttackType = "ProjectN-Win64-Shipping.exe+060FEB70,18,20,0,20";
        private readonly String _addressCharacterMoveType = "ProjectN-Win64-Shipping.exe+0638DAE0,188,38,0,30,2A0,F0";
        private readonly String _addressCharacterMoveType2 = "ProjectN-Win64-Shipping.exe+063818E8,0,20,F0";
        private readonly String _addressCollision = "ProjectN-Win64-Shipping.exe+0638A5A0,DE8,38,0,30,260,5C"; //68 OFF 64 ON
        private readonly String _addressMovementMode = "ProjectN-Win64-Shipping.exe+0638A5A0,DE8,38,0,30,260,288,168"; //1 OFF 5 ON


        private float _coordinateX = 0;
        private float _coordinateY = 0;
        private float _coordinateZ = 0;

        public External()
        {
            InitializeComponent();


            if (!WindowsIdentity.GetCurrent().IsSystem)
            {
                string binaryPath = System.AppDomain.CurrentDomain.BaseDirectory + "NNKCW-External.exe";
                string ProcessToSpoof = "winlogon";
                int parentProcessId;
                Process[] explorerproc = Process.GetProcessesByName(ProcessToSpoof);
                parentProcessId = explorerproc[0].Id;
                RunUnderProcess.Start(parentProcessId, binaryPath);
                Thread.Sleep(1000);
                Environment.Exit(0);
            }

            try
            {
                var open = m.OpenProcess(processName);
                if (open)
                {
                    displayStatus.Text = "Active";
                } else
                {
                    displayStatus.Text = "Ni No Kuni: Cross World not detected";
                }
            } catch(Exception e)
            {
                displayStatus.Text = e.Message;
            }
        }

        public void getPrice(string tokenName)
        {
            HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(string.Format("https://ninokuni.marblex.io/api/price?tokenType=" + tokenName));

            WebReq.Method = "GET";

            HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();

            string jsonString;
            using (Stream stream = WebResp.GetResponseStream())   //modified from your code since the using statement disposes the stream automatically when done
            {
                StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                jsonString = reader.ReadToEnd();
            }

            var json = JsonConvert.DeserializeObject<PriceModel>(jsonString);
            displayLivePriceNKT.Text = json.currencies.USD.priceMajor + "." + json.currencies.USD.priceMinor.Substring(0, 4) + " " + json.currencies.USD.counter;
        }

        public void getExchangeRate(string tokenName)
        {
            HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(string.Format("https://ninokuni.marblex.io/api/exchangeRate?tokenType=" + tokenName));

            WebReq.Method = "GET";

            HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();

            string jsonString;
            using (Stream stream = WebResp.GetResponseStream())   //modified from your code since the using statement disposes the stream automatically when done
            {
                StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                jsonString = reader.ReadToEnd();
            }

            var json = JsonConvert.DeserializeObject<ExchangeRateModel>(jsonString);
            displayExchangeRateNKT.Text = json.result.Last().exchangeRate;
        }

        private void External_Load(object sender, EventArgs e)
        {
            t = new Thread(DoThisAllTheTime);
            t.Start();
        }

        public void DoThisAllTheTime()
        {
            while(true)
            {
                try
                {
                    getPrice("NKT");
                    getExchangeRate("NKT");
                    displayHP.Text = m.ReadInt(_addressHP).ToString();
                    displayCoordinateX.Text = m.ReadFloat(_addressCoordinateX).ToString();
                    displayCoordinateY.Text = m.ReadFloat(_addressCoordinateY).ToString();
                    displayCoordinateZ.Text = m.ReadFloat(_addressCoordinateZ).ToString();
                    displayTimeDilation.Text = m.ReadFloat(_addressTimeDilation).ToString();
                    displayAttackSpeed.Text = m.ReadInt(_addressAttackSpeed).ToString();
                    displayAttackType.Text = m.ReadInt(_addressAttackType).ToString();
                    displayMovementSpeed.Text = m.ReadFloat(_addressMovementSpeed).ToString();
                    displayMultiJumpCount.Text = m.ReadInt(_addressMultiJumpCount).ToString();
                    displayJumpZVelocity.Text = m.ReadFloat(_addressJumpZVelocity).ToString();
                    displayCollision.Text = m.ReadInt(_addressCollision).ToString();
                    displayMovementMode.Text = m.ReadInt(_addressMovementMode).ToString();

                    _coordinateX = m.ReadFloat(_addressCoordinateX);
                    _coordinateY = m.ReadFloat(_addressCoordinateY);
                    _coordinateZ = m.ReadFloat(_addressCoordinateZ);
                } catch(Exception e)
                {
                    //displayStatus.Text = e.ToString();
                }
                Thread.Sleep(1000);
                //you need to use Invoke because the new thread can't access the UI elements directly
                MethodInvoker mi = delegate ()
                {
                };
                this.Invoke(mi);
            }
        }

        private void btnIncreaseX_Click(object sender, EventArgs e)
        {
            m.FreezeValue(_addressCoordinateX, "float", (((int)_coordinateX) + 1000).ToString());
            Thread.Sleep(500);
            m.UnfreezeValue(_addressCoordinateX);
        }

        private void btnIncreaseY_Click(object sender, EventArgs e)
        {
            m.FreezeValue(_addressCoordinateY, "float", (((int)_coordinateY) + 1000).ToString());
            Thread.Sleep(500);
            m.UnfreezeValue(_addressCoordinateY);
        }

        private void btnIncreaseZ_Click(object sender, EventArgs e)
        {
            m.FreezeValue(_addressCoordinateZ, "float", (((int)_coordinateZ) + 400).ToString());
            Thread.Sleep(500);
            m.UnfreezeValue(_addressCoordinateZ);
        }

        private void btnDecreaseX_Click(object sender, EventArgs e)
        {
            m.FreezeValue(_addressCoordinateX, "float", (((int)_coordinateX) - 1000).ToString());
            Thread.Sleep(500);
            m.UnfreezeValue(_addressCoordinateX);
        }

        private void btnDecreaseY_Click(object sender, EventArgs e)
        {
            m.FreezeValue(_addressCoordinateY, "float", (((int)_coordinateY) - 1000).ToString());
            Thread.Sleep(500);
            m.UnfreezeValue(_addressCoordinateY);
        }

        private void btnDecreaseZ_Click(object sender, EventArgs e)
        {
            m.FreezeValue(_addressCoordinateZ, "float", (((int)_coordinateZ) - 400).ToString());
            Thread.Sleep(500);
            m.UnfreezeValue(_addressCoordinateZ);
        }

        private void btnCollision_Click(object sender, EventArgs e)
        {
            if (btnCollision.Text == "ON")
            {
                m.FreezeValue(_addressCollision, "int", "64");
                btnCollision.Text = "OFF";
            } else
            {
                m.FreezeValue(_addressCollision, "int", "68");
                m.UnfreezeValue(_addressCollision);
                btnCollision.Text = "ON";
            }
        }

        private void btnMovementMode_Click(object sender, EventArgs e)
        {
            if (btnMovementMode.Text == "ON")
            {
                m.FreezeValue(_addressMovementMode, "int", "5");
                btnMovementMode.Text = "OFF";
            }
            else
            {
                m.FreezeValue(_addressMovementMode, "int", "1");
                m.UnfreezeValue(_addressMovementMode);
                btnMovementMode.Text = "ON";
            }
        }

        private void btnCancelAnimation_Click(object sender, EventArgs e)
        {
            if (btnCancelAnimation.Text == "ON")
            {
                m.FreezeValue(_addressCancelAnimation, "int", "1");
                btnCancelAnimation.Text = "OFF";
                displayCancelAnimation.Text = "Active";
            }
            else
            {
                m.UnfreezeValue(_addressCancelAnimation);
                btnCancelAnimation.Text = "ON";
                displayCancelAnimation.Text = "Inactive";
            }
        }

        private void activateTimeDilation_CheckedChanged(object sender, EventArgs e)
        {
            if (activateTimeDilation.Checked)
            {
                m.FreezeValue(_addressTimeDilation, "float", inputTimeDilation.Text);
                activateTimeDilation.Text = "Active";
            } else
            {
                m.FreezeValue(_addressTimeDilation, "float", "1");
                Thread.Sleep(500);
                m.UnfreezeValue(_addressTimeDilation);
                activateTimeDilation.Text = "Inactive";
            }
        }

        private void activateAttackSpeed_CheckedChanged(object sender, EventArgs e)
        {
            if (activateAttackSpeed.Checked)
            {
                m.FreezeValue(_addressAttackSpeed, "int", inputAttackSpeed.Text);
                activateAttackSpeed.Text = "Active";
            }
            else
            {
                m.FreezeValue(_addressAttackSpeed, "int", "10000");
                Thread.Sleep(500);
                m.UnfreezeValue(_addressAttackSpeed);
                activateAttackSpeed.Text = "Inactive";
            }
        }

        private void activateAttackType_CheckedChanged(object sender, EventArgs e)
        {
            if (activateAttackType.Checked)
            {
                m.FreezeValue(_addressAttackType, "int", optionAttackType.Text);
                activateAttackType.Text = "Active";
            }
            else
            {
                m.UnfreezeValue(_addressAttackType);
                activateAttackType.Text = "Inactive";
            }
        }

        private void activateMovementSpeed_CheckedChanged(object sender, EventArgs e)
        {
            if (activateMovementSpeed.Checked)
            {
                m.FreezeValue(_addressMovementSpeed, "float", inputMovementSpeed.Text);
                activateMovementSpeed.Text = "Active";
            }
            else
            {
                m.FreezeValue(_addressMovementSpeed, "float", "470");
                Thread.Sleep(500);
                m.UnfreezeValue(_addressMovementSpeed);
                activateMovementSpeed.Text = "Inactive";
            }
        }

        private void activateMultiJumpCount_CheckedChanged(object sender, EventArgs e)
        {
            if (activateMultiJumpCount.Checked)
            {
                m.FreezeValue(_addressMultiJumpCount, "int", inputMultiJumpCount.Text);
                activateMultiJumpCount.Text = "Active";
            }
            else
            {
                m.FreezeValue(_addressMultiJumpCount, "int", "1");
                Thread.Sleep(500);
                m.UnfreezeValue(_addressMultiJumpCount);
                activateMultiJumpCount.Text = "Inactive";
            }
        }

        private void activateJumpZVelocity_CheckedChanged(object sender, EventArgs e)
        {
            if (activateJumpZVelocity.Checked)
            {
                m.FreezeValue(_addressJumpZVelocity, "float", inputJumpZVelocity.Text);
                activateJumpZVelocity.Text = "Active";
            }
            else
            {
                m.FreezeValue(_addressJumpZVelocity, "float", "450");
                Thread.Sleep(500);
                m.UnfreezeValue(_addressJumpZVelocity);
                activateJumpZVelocity.Text = "Inactive";
            }
        }

        private void inputTimeDilation_TextChanged(object sender, EventArgs e)
        {
            if (activateTimeDilation.Checked)
            {
                m.UnfreezeValue(_addressTimeDilation);
                m.FreezeValue(_addressTimeDilation, "float", inputTimeDilation.Text);
            }
        }

        private void inputAttackSpeed_TextChanged(object sender, EventArgs e)
        {
            if (activateAttackSpeed.Checked)
            {
                m.UnfreezeValue(_addressAttackSpeed);
                m.FreezeValue(_addressAttackSpeed, "int", inputAttackSpeed.Text);
            }
        }

        private void optionAttackType_TextChanged(object sender, EventArgs e)
        {
            if (activateAttackType.Checked)
            {
                m.UnfreezeValue(_addressAttackType);
                m.FreezeValue(_addressAttackType, "int", optionAttackType.Text);
            }
        }

        private void inputMovementSpeed_TextChanged(object sender, EventArgs e)
        {
            if (activateMovementSpeed.Checked)
            {
                m.UnfreezeValue(_addressMovementSpeed);
                m.FreezeValue(_addressMovementSpeed, "float", inputMovementSpeed.Text);
            }
        }

        private void inputMultiJumpCount_TextChanged(object sender, EventArgs e)
        {
            if (activateMultiJumpCount.Checked)
            {
                m.UnfreezeValue(_addressMultiJumpCount);
                m.FreezeValue(_addressMultiJumpCount, "int", inputMultiJumpCount.Text);
            }
        }

        private void inputJumpZVelocity_TextChanged(object sender, EventArgs e)
        {
            if (activateJumpZVelocity.Checked)
            {
                m.UnfreezeValue(_addressJumpZVelocity);
                m.FreezeValue(_addressJumpZVelocity, "float", inputJumpZVelocity.Text);
            }
        }
    }
}
