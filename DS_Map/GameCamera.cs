using System;
using System.IO;
using System.Windows.Forms;

public class GameCamera {

    public const byte PERSPECTIVE = 0;
    public const byte ORTHO = 1;

    public uint distance { get; set; }

    public short vertRot { get; private set; }
    public short horiRot { get; private set; }
    public short zRot { get; private set; }
    public short unk1 { get; private set; }

    public byte perspMode { get; private set; }

    public byte unk2 { get; private set; }


    public ushort fov { get; private set; }
    public uint nearClip { get; private set; }
    public uint farClip { get; private set; }

    public int? xOffset { get; private set; }
    public int? yOffset { get; private set; }
    public int? zOffset { get; private set; }

    public object this[int index] {
        get {
            switch (index) {
                case 0:
                    return distance;
                case 1:
                    return vertRot;
                case 2:
                    return horiRot;
                case 3:
                    return zRot;
                case 5:
                    return perspMode;
                case 6:
                    return fov;
                case 7:
                    return nearClip;
                case 8:
                    return farClip;
                case 9:
                    return xOffset;
                case 10:
                    return yOffset;
                case 11:
                    return zOffset;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        set {
            switch (index) {
                case 0:
                    distance = Convert.ToUInt32(value);
                    break;
                case 1:
                    vertRot = Convert.ToInt16(value);
                    break;
                case 2:
                    horiRot = Convert.ToInt16(value);
                    break;
                case 3:
                    zRot = Convert.ToInt16(value);
                    break;
                case 4:
                    perspMode = (byte)(Convert.ToBoolean(value) ? 1 : 0);
                    break;
                case 5:
                    fov = Convert.ToUInt16(value);
                    break;
                case 6:
                    nearClip = Convert.ToUInt32(value);
                    break;
                case 7:
                    farClip = Convert.ToUInt32(value);
                    break;
                case 8:
                    xOffset = Convert.ToInt32(value);
                    break;
                case 9:
                    yOffset = Convert.ToInt32(value);
                    break;
                case 10:
                    zOffset = Convert.ToInt32(value);
                    break;

            }
        }
    }

    public GameCamera(uint distance = 0x29AEC1, short vertRot = 0xD62, short horiRot = 0, short zRot = 0,
                        short unk1 = 0, byte perspMode = PERSPECTIVE, byte unk2 = 0,
                        ushort fov = 1473, uint nearClip = 614400, uint farClip = 0x384000,
                        int? xOffset = null, int? yOffset = null, int? zOffset = null) {

        this.distance = distance;
        this.vertRot = vertRot;
        this.horiRot = horiRot;

        this.unk1 = unk1;
        this.perspMode = perspMode;

        this.fov = fov;
        this.nearClip = nearClip;
        this.farClip = farClip;

        this.xOffset = xOffset;
        this.yOffset = yOffset;
        this.zOffset = zOffset;
    }
    public byte[] ToByteArray() {
        MemoryStream newData = new MemoryStream();
        using (BinaryWriter writer = new BinaryWriter(newData)) {
            writer.Write(distance);
            writer.Write(vertRot);
            writer.Write(horiRot);
            writer.Write(zRot);

            writer.Write(unk1);
            writer.Write(perspMode);
            writer.Write(unk2);

            writer.Write(fov);
            writer.Write(nearClip);
            writer.Write(farClip);

            if (xOffset != null)
                writer.Write((int)xOffset);
            if (yOffset != null)
                writer.Write((int)yOffset);
            if (zOffset != null)
                writer.Write((int)zOffset);
        }

        return newData.ToArray();
    }

    public void AddToGridView(DataGridView dgv) {
        dgv.Rows.Add();
        int cRows = dgv.RowCount;
        int cellIndex = 0;

        dgv.Rows[cRows-1].HeaderCell.Value = String.Format("{0}", dgv.Rows[cRows - 1].Index);

        dgv.Rows[cRows - 1].Cells[cellIndex++].Value = distance;
        dgv.Rows[cRows - 1].Cells[cellIndex++].Value = vertRot;
        dgv.Rows[cRows - 1].Cells[cellIndex++].Value = horiRot;
        dgv.Rows[cRows - 1].Cells[cellIndex++].Value = zRot;

        dgv.Rows[cRows - 1].Cells[cellIndex++].Value = perspMode == ORTHO;

        dgv.Rows[cRows - 1].Cells[cellIndex++].Value = fov;
        dgv.Rows[cRows - 1].Cells[cellIndex++].Value = nearClip;
        dgv.Rows[cRows - 1].Cells[cellIndex++].Value = farClip;

        if (xOffset != null)
            dgv.Rows[cRows - 1].Cells[cellIndex++].Value = xOffset;
        if (yOffset != null)
            dgv.Rows[cRows - 1].Cells[cellIndex++].Value = yOffset;
        if (zOffset != null)
            dgv.Rows[cRows - 1].Cells[cellIndex++].Value = zOffset;
    }
}
