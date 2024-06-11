using System.IO;

namespace DSPRE.ROMFiles {
    public class HeadbuttTree {
        public bool picked = false;

        private ushort _globalX;
        private ushort _globalY;
        private ushort _matrixX;
        private ushort _matrixY;
        private ushort _mapX;
        private ushort _mapY;

        public bool IsUnused { 
            get { 
                return globalX == ushort.MaxValue && globalY == ushort.MaxValue; 
            } 
        }

        public enum Types {
            Normal,
            Special,
        }

        public HeadbuttTree(ushort globalX = ushort.MaxValue, ushort globalY = ushort.MaxValue) {
            this.globalX = globalX;
            this.globalY = globalY;
        }

        public HeadbuttTree(HeadbuttTree original) {
            this.globalX = original.globalX;
            this.globalY = original.globalY;
        }

        public ushort globalX {
            get { return _globalX; }
            set {
                _globalX = value;
                _matrixX = (ushort)(_globalX / MapFile.mapSize);
                _mapX = (ushort)(_globalX % MapFile.mapSize);
            }
        }

        public ushort globalY {
            get { return _globalY; }
            set {
                _globalY = value;
                _matrixY = (ushort)(_globalY / MapFile.mapSize);
                _mapY = (ushort)(_globalY % MapFile.mapSize);
            }
        }

        public ushort matrixX {
            get {
                return _matrixX;
            }
            set {
                _matrixX = value;
                _globalX = (ushort)(_matrixX * MapFile.mapSize + _mapX);
                _mapX = (ushort)(_globalX % MapFile.mapSize);
            }
        }

        public ushort matrixY {
            get {
                return _matrixY;
            }
            set {
                _matrixY = value;
                _globalY = (ushort)(_matrixY * MapFile.mapSize + _mapY);
                _mapY = (ushort)(_globalY % MapFile.mapSize);
            }
        }

        public ushort mapX {
            get {
                return _mapX;
            }
            set {
                _mapX = value;
                _globalX = (ushort)(_matrixX * MapFile.mapSize + _mapX);
                _matrixX = (ushort)(_globalX / MapFile.mapSize);
            }
        }

        public ushort mapY {
            get {
                return _mapY;
            }
            set {
                _mapY = value;
                _globalY = (ushort)(_matrixY * MapFile.mapSize + _mapY);
                _matrixY = (ushort)(_globalY / MapFile.mapSize);
            }
        }

        public override string ToString() {
            if (IsUnused) {
                return "Unused Tree";
            }
            return $"Tree at Global X: {globalX}, Global Y: {globalY}";
        }
    }
}
