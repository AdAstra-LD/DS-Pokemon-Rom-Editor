using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using static DSPRE.RomInfo;

namespace DSPRE.ROMFiles {
    /// <summary>
    /// Classes to store event data in Pokémon NDS games
    /// </summary>
    public class EventFile : RomFile {
        #region Fields
        public List<Spawnable> spawnables = new List<Spawnable>();
        public List<Overworld> overworlds = new List<Overworld>();
        public List<Warp> warps = new List<Warp>();
        public List<Trigger> triggers = new List<Trigger>();
        #endregion

        #region Constructors (1)
        public EventFile(Stream data) {
            using (BinaryReader reader = new BinaryReader(data)) {
                /* Read spawnables */
                uint spawnablesCount = reader.ReadUInt32();
                for (int i = 0; i < spawnablesCount; i++)
                    spawnables.Add(new Spawnable(new MemoryStream(reader.ReadBytes(0x14))));

                /* Read overworlds */
                uint overworldsCount = reader.ReadUInt32();
                for (int i = 0; i < overworldsCount; i++)
                    overworlds.Add(new Overworld(new MemoryStream(reader.ReadBytes(0x20))));

                /* Read warps */
                uint warpsCount = reader.ReadUInt32();
                for (int i = 0; i < warpsCount; i++)
                    warps.Add(new Warp(new MemoryStream(reader.ReadBytes(0xC))));

                /* Read triggers */
                uint triggersCount = reader.ReadUInt32();
                for (int i = 0; i < triggersCount; i++)
                    triggers.Add(new Trigger(new MemoryStream(reader.ReadBytes(0x10))));
            }
        }
        public EventFile(int ID) : this(new FileStream(RomInfo.gameDirs[DirNames.eventFiles].unpackedDir + "\\" + ID.ToString("D4"), FileMode.Open)) { }
        #endregion

        #region Methods (1)
        public override byte[] ToByteArray() {
            MemoryStream newData = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(newData)) {
                /* Write spawnables */
                writer.Write((uint)spawnables.Count);
                for (int i = 0; i < spawnables.Count; i++)
                    writer.Write(spawnables[i].ToByteArray());

                /* Write overworlds */
                writer.Write((uint)overworlds.Count);
                for (int i = 0; i < overworlds.Count; i++)
                    writer.Write(overworlds[i].ToByteArray());

                /* Write warps */
                writer.Write((uint)warps.Count);
                for (int i = 0; i < warps.Count; i++)
                    writer.Write(warps[i].ToByteArray());

                /* Write triggers */
                writer.Write((uint)triggers.Count);
                for (int i = 0; i < triggers.Count; i++)
                    writer.Write(triggers[i].ToByteArray());
            }
            return newData.ToArray();
        }
        public void SaveToFileDefaultDir(int IDtoReplace, bool showSuccessMessage = true) {
            SaveToFileDefaultDir(DirNames.eventFiles, IDtoReplace, showSuccessMessage);
        }
        public void SaveToFileExplorePath(string suggestedFileName, bool showSuccessMessage = true) {
            SaveToFileExplorePath("Gen IV Event File", "evt", suggestedFileName, showSuccessMessage);
        }
        #endregion

    }

    public abstract class Event {
        public enum EventType : byte {
            SPAWNABLE,
            OVERWORLD,
            WARP,
            TRIGGER
        }
        #region Fields (6)
        public EventType evType;

        public short xMapPosition;
        public short yMapPosition;
        public short zPosition;
        public ushort xMatrixPosition;
        public ushort yMatrixPosition;
        #endregion

        #region Methods (1)
        public abstract byte[] ToByteArray();
        #endregion
    }

    public class Spawnable : Event {
        public const int TYPE_MISC = 0;
        public const int TYPE_BOARD = 1;
        public const int TYPE_HIDDENITEM = 2;

        #region Fields (7)
        public ushort scriptNumber;
        public ushort type;
        public ushort unknown2;
        public ushort unknown3;
        public ushort unknown4;
        public ushort dir;
        public ushort unknown5;
        #endregion

        #region Constructors (2)
        public Spawnable(Stream data) {
            evType = EventType.SPAWNABLE;
            using (BinaryReader reader = new BinaryReader(data)) {
                scriptNumber = reader.ReadUInt16();
                type = reader.ReadUInt16();

                /* Decompose x coordinate in matrix and map positions */
                int xPosition = reader.ReadInt16();
                xMapPosition = (short)(xPosition % MapFile.mapSize);
                xMatrixPosition = (ushort)(xPosition / MapFile.mapSize);

                unknown2 = reader.ReadUInt16();

                /* Decompose y coordinate in matrix and map positions */
                int yPosition = reader.ReadInt16();
                yMapPosition = (short)(yPosition % MapFile.mapSize);
                yMatrixPosition = (ushort)(yPosition / MapFile.mapSize);

                unknown3 = reader.ReadUInt16();
                zPosition = reader.ReadInt16();
                unknown4 = reader.ReadUInt16();
                dir = reader.ReadUInt16();
                unknown5 = reader.ReadUInt16();
            }
        }
        public Spawnable(int xMatrixPosition, int yMatrixPosition) {
            evType = EventType.SPAWNABLE;

            scriptNumber = 0;
            type = 0;
            unknown2 = 0;
            unknown3 = 0;
            unknown4 = 0;
            unknown5 = 0;
            dir = 0;

            xMapPosition = 0;
            yMapPosition = 0;
            zPosition = 0;
            this.xMatrixPosition = (ushort)xMatrixPosition;
            this.yMatrixPosition = (ushort)yMatrixPosition;
        }
        public Spawnable(Spawnable toCopy) {
            evType = EventType.SPAWNABLE;

            scriptNumber = toCopy.scriptNumber;
            type = toCopy.type;
            unknown2 = toCopy.unknown2;
            unknown3 = toCopy.unknown3;
            unknown4 = toCopy.unknown4;
            unknown5 = toCopy.unknown5;
            dir = toCopy.dir;

            xMapPosition = toCopy.xMapPosition;
            yMapPosition = toCopy.yMapPosition;
            zPosition = toCopy.zPosition;
            this.xMatrixPosition = toCopy.xMatrixPosition;
            this.yMatrixPosition = toCopy.yMatrixPosition;
        }
        #endregion

        #region Methods (1)
        public override byte[] ToByteArray() {
            using (BinaryWriter writer = new BinaryWriter(new MemoryStream())) {
                writer.Write(scriptNumber);
                writer.Write(type);
                short xCoordinate = (short)(xMapPosition + MapFile.mapSize * xMatrixPosition);
                writer.Write(xCoordinate);
                writer.Write(unknown2);
                short yCoordinate = (short)(yMapPosition + MapFile.mapSize * yMatrixPosition);
                writer.Write(yCoordinate);
                writer.Write(unknown3);
                writer.Write(zPosition);
                writer.Write(unknown4);
                writer.Write(dir);
                writer.Write(unknown5);

                return ((MemoryStream)writer.BaseStream).ToArray();
            }
        }
        #endregion
    }

    public class Overworld : Event {
        #region Fields (14)
        public enum owType : ushort { NORMAL = 0, TRAINER = 1, ITEM = 3 };

        public ushort owID;
        public ushort overlayTableEntry;
        public ushort movement;
        public ushort type;
        public ushort flag;
        public ushort scriptNumber;
        public ushort orientation;
        public ushort sightRange;
        public ushort unknown1;
        public ushort unknown2;
        public ushort xRange;
        public ushort yRange;
        public ushort unknown3;
        public bool is3D = new bool();
        #endregion

        #region Constructors (2)
        public Overworld(Stream data) {
            evType = EventType.OVERWORLD;
            using (BinaryReader reader = new BinaryReader(data)) {
                owID = reader.ReadUInt16();
                overlayTableEntry = reader.ReadUInt16();
                movement = reader.ReadUInt16();
                type = reader.ReadUInt16();
                flag = reader.ReadUInt16();
                scriptNumber = reader.ReadUInt16();
                orientation = reader.ReadUInt16();
                sightRange = reader.ReadUInt16();
                unknown1 = reader.ReadUInt16();
                unknown2 = reader.ReadUInt16();
                xRange = reader.ReadUInt16();
                yRange = reader.ReadUInt16();

                /* Decompose x-y coordinates in matrix and map positions */
                int xPosition = reader.ReadInt16();
                int yPosition = reader.ReadInt16();
                xMapPosition = (short)(xPosition % MapFile.mapSize);
                yMapPosition = (short)(yPosition % MapFile.mapSize);
                xMatrixPosition = (ushort)(xPosition / MapFile.mapSize);
                yMatrixPosition = (ushort)(yPosition / MapFile.mapSize);

                zPosition = reader.ReadInt16();
                unknown3 = reader.ReadUInt16();
            }
        }
        public Overworld(int owID, int xMatrixPosition, int yMatrixPosition) {
            evType = EventType.OVERWORLD;

            this.owID = (ushort)owID;
            overlayTableEntry = 1;
            movement = 0;
            type = 0;
            flag = 0;
            scriptNumber = 0;
            orientation = 1;
            sightRange = 0;
            unknown1 = 0;
            unknown2 = 0;
            xRange = 0;
            yRange = 0;
            unknown3 = 0;

            xMapPosition = 16;
            yMapPosition = 16;
            zPosition = 0;
            this.xMatrixPosition = (ushort)xMatrixPosition;
            this.yMatrixPosition = (ushort)yMatrixPosition;
        }
        public Overworld(Overworld toCopy) {
            evType = EventType.OVERWORLD;

            this.owID = toCopy.owID;
            overlayTableEntry = toCopy.overlayTableEntry;
            movement = toCopy.movement;
            type = toCopy.type;
            flag = toCopy.flag;
            scriptNumber = toCopy.scriptNumber;
            orientation = toCopy.orientation;
            sightRange = toCopy.sightRange;
            unknown1 = toCopy.unknown1;
            unknown2 = toCopy.unknown2;
            xRange = toCopy.xRange;
            yRange = toCopy.yRange;
            unknown3 = toCopy.unknown3;

            xMapPosition = toCopy.xMapPosition;
            yMapPosition = toCopy.yMapPosition;
            zPosition = toCopy.zPosition;
            this.xMatrixPosition = toCopy.xMatrixPosition;
            this.yMatrixPosition = toCopy.yMatrixPosition;
        }
        #endregion

        #region Methods (1)
        public override byte[] ToByteArray() {
            using (BinaryWriter writer = new BinaryWriter(new MemoryStream())) {
                writer.Write(owID);
                writer.Write(overlayTableEntry);
                writer.Write(movement);
                writer.Write(type);
                writer.Write(flag);
                writer.Write(scriptNumber);
                writer.Write(orientation);
                writer.Write(sightRange);
                writer.Write(unknown1);
                writer.Write(unknown2);
                writer.Write(xRange);
                writer.Write(yRange);

                short xCoordinate = (short)(xMapPosition + MapFile.mapSize * xMatrixPosition);
                writer.Write(xCoordinate);

                short yCoordinate = (short)(yMapPosition + MapFile.mapSize * yMatrixPosition);
                writer.Write(yCoordinate);

                writer.Write(zPosition);
                writer.Write(unknown3);

                return ((MemoryStream)writer.BaseStream).ToArray();
            }
        }
        #endregion

    }

    public class Warp : Event {
        #region Fields (4)
        public ushort header;
        public ushort anchor;
        public uint height;
        #endregion

        #region Constructors (2)
        public Warp(Stream data) {
            evType = EventType.WARP;
            using (BinaryReader reader = new BinaryReader(data)) {
                /* Decompose x-y coordinates in matrix and map positions */
                int xPosition = reader.ReadInt16();
                int yPosition = reader.ReadInt16();
                xMapPosition = (short)(xPosition % MapFile.mapSize);
                yMapPosition = (short)(yPosition % MapFile.mapSize);
                xMatrixPosition = (ushort)(xPosition / MapFile.mapSize);
                yMatrixPosition = (ushort)(yPosition / MapFile.mapSize);

                header = reader.ReadUInt16();
                anchor = reader.ReadUInt16();
                height = reader.ReadUInt32();
            }
        }
        public Warp(int xMatrixPosition, int yMatrixPosition) {
            evType = EventType.WARP;

            header = 0;
            anchor = 0;

            xMapPosition = 0;
            yMapPosition = 0;
            this.xMatrixPosition = (ushort)xMatrixPosition;
            this.yMatrixPosition = (ushort)yMatrixPosition;
        }
        public Warp(Warp toCopy) {
            evType = EventType.WARP;

            header = toCopy.header;
            anchor = toCopy.anchor;

            xMapPosition = toCopy.xMapPosition;
            yMapPosition = toCopy.yMapPosition;
            this.xMatrixPosition = toCopy.xMatrixPosition;
            this.yMatrixPosition = toCopy.yMatrixPosition;
        }
        #endregion

        #region Methods (1)
        public override byte[] ToByteArray() {
            using (BinaryWriter writer = new BinaryWriter(new MemoryStream())) {
                ushort xCoordinate = (ushort)(xMapPosition + MapFile.mapSize * xMatrixPosition);
                writer.Write(xCoordinate);

                ushort yCoordinate = (ushort)(yMapPosition + MapFile.mapSize * yMatrixPosition);
                writer.Write(yCoordinate);

                writer.Write(header);
                writer.Write(anchor);
                writer.Write(height);

                return ((MemoryStream)writer.BaseStream).ToArray();
            }
        }
        #endregion

    }

    public class Trigger : Event {
        #region Fields (7)
        public ushort scriptNumber;
        public ushort widthX;
        public ushort heightY;
        new public ushort zPosition;
        public ushort expectedVarValue;
        public ushort variableWatched;
        #endregion Fields

        #region Constructors (2)
        public Trigger(Stream data) {
            evType = EventType.TRIGGER;
            using (BinaryReader reader = new BinaryReader(data)) {
                scriptNumber = reader.ReadUInt16();

                /* Decompose x-y coordinates in matrix and map positions */
                int xPosition = reader.ReadInt16();
                int yPosition = reader.ReadInt16();
                xMapPosition = (short)(xPosition % MapFile.mapSize);
                yMapPosition = (short)(yPosition % MapFile.mapSize);
                xMatrixPosition = (ushort)(xPosition / MapFile.mapSize);
                yMatrixPosition = (ushort)(yPosition / MapFile.mapSize);

                widthX = reader.ReadUInt16();
                heightY = reader.ReadUInt16();

                zPosition = reader.ReadUInt16();
                expectedVarValue = reader.ReadUInt16();
                variableWatched = reader.ReadUInt16();
            }
        }
        public Trigger(int xMatrixPosition, int yMatrixPosition) {
            evType = EventType.TRIGGER;

            scriptNumber = 0;
            variableWatched = 0;
            expectedVarValue = 0;
            widthX = 1;
            heightY = 1;

            xMapPosition = 0;
            yMapPosition = 0;
            this.xMatrixPosition = (ushort)xMatrixPosition;
            this.yMatrixPosition = (ushort)yMatrixPosition;
        }
        public Trigger(Trigger toCopy) {
            evType = EventType.TRIGGER;

            scriptNumber = toCopy.scriptNumber;
            variableWatched = toCopy.variableWatched;
            expectedVarValue = toCopy.expectedVarValue;
            widthX = toCopy.widthX;
            heightY = toCopy.heightY;

            xMapPosition = toCopy.xMapPosition;
            yMapPosition = toCopy.xMapPosition;
            this.xMatrixPosition = toCopy.xMatrixPosition;
            this.yMatrixPosition = toCopy.yMatrixPosition;
        }
        #endregion

        #region Methods (1)
        public override byte[] ToByteArray() {
            using (BinaryWriter writer = new BinaryWriter(new MemoryStream())) {
                writer.Write(scriptNumber);
                ushort xCoordinate = (ushort)(xMapPosition + MapFile.mapSize * xMatrixPosition);
                writer.Write(xCoordinate);
                ushort yCoordinate = (ushort)(yMapPosition + MapFile.mapSize * yMatrixPosition);
                writer.Write(yCoordinate);
                writer.Write(widthX);
                writer.Write(heightY);
                writer.Write(zPosition);
                writer.Write(expectedVarValue);
                writer.Write(variableWatched);

                return ((MemoryStream)writer.BaseStream).ToArray();
            }
        }
        #endregion
    }
}