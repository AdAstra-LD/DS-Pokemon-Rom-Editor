using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using static DSPRE.DSUtils;

namespace DSPRE
{
    public partial class AddressHelper : Form
    {

        private int overlaysSize = OverlayUtils.OverlayTable.GetNumberOfOverlays();
        private int ARM9LoadAddress = 0x02000000;

        public AddressHelper()
        {
            InitializeComponent();
        }

        private void searchAddressButton_Click(object sender, EventArgs e)
        {
            addressesGrid.Rows.Clear();
            try
            {
                int convertedAddress = Convert.ToInt32(AddressInput.Text, 16);
                List<int> foundInOvl = getOverlayNumberFromAddress(convertedAddress);
                for (int i = 0; i < foundInOvl.Count; i++)
                {
                    int ovl = foundInOvl[i];
                    addressesGrid.Rows.Add("Overlay " + ovl, getOffsetInOverlay(convertedAddress, ovl));
                }


                bool addressToARMLoad = convertedAddress >= ARM9LoadAddress;
                bool addressToARMEnd = convertedAddress < OverlayUtils.OverlayTable.GetRAMAddress(0);

                if (addressToARMLoad && addressToARMEnd)
                {
                    addressesGrid.Rows.Clear();
                    addressesGrid.Rows.Add("ARM9", $"0x{(convertedAddress - ARM9LoadAddress):X4}");
                }



                if (convertedAddress >= RomInfo.synthOverlayLoadAddress)
                {
                    addressesGrid.Rows.Add("SynthOVL", $"0x{convertedAddress - RomInfo.synthOverlayLoadAddress:X4}");
                }
            }
            catch
            {
                MessageBox.Show("No overlay found");
            }
        }

        private List<int> getOverlayNumberFromAddress(int address)
        {
            List<int> overlayNumbers = new List<int>();


            for (int i = 0; i < overlaysSize - 1; i++)
            {
                uint currentOvlAddress = OverlayUtils.OverlayTable.GetRAMAddress(i);
                bool checkOverlayN = currentOvlAddress >= address;
                bool checkOverlayN1 = address < (currentOvlAddress + OverlayUtils.OverlayTable.GetUncompressedSize(i));

                if (checkOverlayN && checkOverlayN1)
                {
                    overlayNumbers.Add(i);
                }
            }


            return overlayNumbers;
        }

        private string getOffsetInOverlay(int address, int ovlNumber)
        {
            return $"0x{OverlayUtils.OverlayTable.GetRAMAddress(ovlNumber) - address:X4}";
        }

    }


}