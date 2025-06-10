using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPRE.Editors.Data
{
    public class FlyTableRowDpPlat
    {
        public FlyTableRowDpPlat() { }

        public FlyTableRowDpPlat(ushort headerIdGameOver, ushort localX, ushort localY, ushort headerIdFly, ushort globalX, ushort globalY,
                                 byte isTeleportPos, byte unlockOnMapEntry, ushort unlockId)
        {
            HeaderIdGameOver = headerIdGameOver;
            LocalX = localX;
            LocalY = localY;
            HeaderIdFly = headerIdFly;
            GlobalX = globalX;
            GlobalY = globalY;
            IsTeleportPos = isTeleportPos;
            UnlockOnMapEntry = unlockOnMapEntry;
            UnlockId = unlockId;
        }

        public ushort GlobalX { get; set; }
        public ushort GlobalY { get; set; }
        public ushort HeaderIdFly { get; set; }
        public ushort HeaderIdGameOver { get; set; }
        public byte IsTeleportPos { get; set; }
        public ushort LocalX { get; set; }
        public ushort LocalY { get; set; }
        public ushort UnlockId { get; set; }
        public byte UnlockOnMapEntry { get; set; }
    }

}
