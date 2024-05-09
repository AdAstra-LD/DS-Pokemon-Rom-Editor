using System.IO;

namespace DSPRE {
    public static class Filesystem {
        public static string eventFiles { get { return RomInfo.gameDirs[RomInfo.DirNames.eventFiles].unpackedDir; } }
        public static string OWSprites { get { return RomInfo.gameDirs[RomInfo.DirNames.OWSprites].unpackedDir; } }
        public static string mapTextures { get { return RomInfo.gameDirs[RomInfo.DirNames.mapTextures].unpackedDir; } }
        public static string buildingTextures { get { return RomInfo.gameDirs[RomInfo.DirNames.buildingTextures].unpackedDir; } }
        public static string dynamicHeaders { get { return RomInfo.gameDirs[RomInfo.DirNames.dynamicHeaders].unpackedDir; } }
        public static string dynamicHeadersPacked { get { return RomInfo.gameDirs[RomInfo.DirNames.dynamicHeaders].packedDir; } }
        public static string scripts { get { return RomInfo.gameDirs[RomInfo.DirNames.scripts].unpackedDir; } }
        public static string maps { get { return RomInfo.gameDirs[RomInfo.DirNames.maps].unpackedDir; } }
        public static string matrices { get { return RomInfo.gameDirs[RomInfo.DirNames.matrices].unpackedDir; } }
        public static string buildingConfigFiles { get { return RomInfo.gameDirs[RomInfo.DirNames.buildingConfigFiles].unpackedDir; } }
        public static string areaData { get { return RomInfo.gameDirs[RomInfo.DirNames.areaData].unpackedDir; } }
        public static string textArchives { get { return RomInfo.gameDirs[RomInfo.DirNames.textArchives].unpackedDir; } }
        public static string trainerProperties { get { return RomInfo.gameDirs[RomInfo.DirNames.trainerProperties].unpackedDir; } }
        public static string trainerParty { get { return RomInfo.gameDirs[RomInfo.DirNames.trainerParty].unpackedDir; } }
        public static string trainerGraphics { get { return RomInfo.gameDirs[RomInfo.DirNames.trainerGraphics].unpackedDir; } }
        public static string encounters { get { return RomInfo.gameDirs[RomInfo.DirNames.encounters].unpackedDir; } }
        public static string headbutt { get { return RomInfo.gameDirs[RomInfo.DirNames.headbutt].unpackedDir; } }
        public static string safariZone { get { return RomInfo.gameDirs[RomInfo.DirNames.safariZone].unpackedDir; } }
        public static string monIcons { get { return RomInfo.gameDirs[RomInfo.DirNames.monIcons].unpackedDir; } }
        public static string synthOverlay { get { return RomInfo.gameDirs[RomInfo.DirNames.synthOverlay].unpackedDir; } }
        public static string interiorBuildingModels { get { return RomInfo.gameDirs[RomInfo.DirNames.interiorBuildingModels].unpackedDir; } }
        public static string exteriorBuildingModels { get { return RomInfo.gameDirs[RomInfo.DirNames.exteriorBuildingModels].unpackedDir; } }

        public static string GetBuildingModelsDirPath(bool interior) {
            if (interior) {
                return interiorBuildingModels;
            } else {
                return exteriorBuildingModels;
            }
        }

        public static string expArmPath { get { return Path.Combine(synthOverlay, PatchToolboxDialog.expandedARMfileID.ToString("D4")); } }

        public static string GetPath(string path, int id, string format = "D4") {
            return Path.Combine(path, id.ToString(format));
        }

        public static string GetPath(string path, string prefix, int id, string ext, string format = "D4") {
            return Path.Combine(path, prefix + id.ToString(format) + "." + ext);
        }

        static string[] GetBuildingModelFiles(bool interior) {
            return Directory.GetFiles(Filesystem.GetBuildingModelsDirPath(interior));
        }

        public static string GetBuildingModelPath(bool interior, int id) {
            return GetPath(Filesystem.GetBuildingModelsDirPath(interior), id);
        }

        public static int GetBuildingCount(bool interior) {
            return GetBuildingModelFiles(interior).Length;
        }

        public static string[] GetAreaDataFiles() {
            return Directory.GetFiles(Filesystem.areaData);
        }

        public static string GetAreaDataPath(int id) {
            return GetPath(Filesystem.areaData, id);
        }

        public static int GetAreaDataCount() {
            return GetAreaDataFiles().Length;
        }

        public static string GetTexturePath(bool useMapTiles, int textureID) {
            string path = Filesystem.GetMapTexturePath(textureID);
            string path2 = Filesystem.GetBuildingTexturePath(textureID);
            string tilesetPath = useMapTiles ? path : path2;
            return tilesetPath;
        }

        static string[] GetMapTextureFiles() {
            return Directory.GetFiles(Filesystem.mapTextures);
        }

        public static string GetMapTexturePath(int id) {
            return GetPath(Filesystem.mapTextures, id);
        }

        public static int GetMapTexturesCount() {
            return GetMapTextureFiles().Length;
        }

        static string[] GetBuildingTextureFiles() {
            return Directory.GetFiles(Filesystem.buildingTextures);
        }

        public static string GetBuildingTexturePath(int id) {
            return GetPath(Filesystem.buildingTextures, id);
        }

        public static int GetBuildingTexturesCount() {
            return GetBuildingTextureFiles().Length;
        }

        static string[] GetMatrixFiles() {
            return Directory.GetFiles(Filesystem.matrices);
        }

        public static string GetMatrixPath(int id) {
            return GetPath(Filesystem.matrices, id);
        }

        public static int GetMatrixCount() {
            return GetMatrixFiles().Length;
        }

        static string[] GetTextArchiveFiles() {
            return Directory.GetFiles(Filesystem.textArchives);
        }

        public static string GetTextArchivePath(int id) {
            return GetPath(Filesystem.textArchives, id);
        }

        public static int GetTextArchivesCount() {
            return GetTextArchiveFiles().Length;
        }

        static string[] GetMapFiles() {
            return Directory.GetFiles(Filesystem.maps);
        }

        public static string GetMapPath(int id) {
            return GetPath(Filesystem.maps, id);
        }

        public static int GetMapCount() {
            return GetMapFiles().Length;
        }

        static string[] GetScriptFiles() {
            return Directory.GetFiles(Filesystem.scripts);
        }

        public static string GetScriptPath(int id) {
            return GetPath(Filesystem.scripts, id);
        }

        public static int GetScriptCount() {
            return GetScriptFiles().Length;
        }

        static string[] GetEventFiles() {
            return Directory.GetFiles(Filesystem.eventFiles);
        }

        public static string GetEventPath(int id) {
            return GetPath(Filesystem.eventFiles, id);
        }

        public static int GetEventFileCount() {
            return GetEventFiles().Length;
        }

        static string[] GetTrainerPropertiesFiles() {
            return Directory.GetFiles(Filesystem.trainerProperties);
        }

        public static string GetTrainerPropertiesPath(int id) {
            return GetPath(Filesystem.trainerProperties, id);
        }

        public static int GetTrainerPropertiesCount() {
            return GetTrainerPropertiesFiles().Length;
        }

        static string[] GetDynamicHeaderFiles() {
            return Directory.GetFiles(Filesystem.dynamicHeaders);
        }

        public static string GetDynamicHeaderPath(int id) {
            return GetPath(Filesystem.dynamicHeaders, id);
        }

        public static int GetDynamicHeadersCount() {
            return GetDynamicHeaderFiles().Length;
        }

        static string[] GetEncounterFiles() {
            return Directory.GetFiles(Filesystem.encounters);
        }

        public static string GetEncounterPath(int id) {
            return GetPath(Filesystem.encounters, id);
        }

        public static int GetEncountersCount() {
            return GetEncounterFiles().Length;
        }

        static string[] GetSafariZoneFiles() {
            return Directory.GetFiles(Filesystem.safariZone);
        }

        public static string GetSafariZonePath(int id) {
            return GetPath(Filesystem.safariZone, id);
        }

        public static int GetSafariZoneCount() {
            return GetSafariZoneFiles().Length;
        }

        static string[] GetHeadbuttFiles() {
            return Directory.GetFiles(Filesystem.headbutt);
        }

        public static string GetHeadbuttPath(int id) {
            return GetPath(Filesystem.headbutt, id);
        }

        public static int GetHeadbuttCount() {
            return GetHeadbuttFiles().Length;
        }

        public static string GetOWSpritePath(int id) {
            return GetPath(Filesystem.OWSprites, id);
        }

        public static string GetBuildingConfigPath(int id) {
            return GetPath(Filesystem.buildingConfigFiles, id);
        }

        public static string GetTrainerPartyPath(int id) {
            return GetPath(Filesystem.trainerParty, id);
        }

        public static string GetTrainerGraphicsPath(int id) {
            return GetPath(Filesystem.trainerGraphics, id);
        }

        public static string GetMonIconPath(int id, string format = "D4") {
            return GetPath(Filesystem.monIcons, id, format);
        }

        public static string GetSynthOerlayPath(int id) {
            return GetPath(Filesystem.synthOverlay, id);
        }
    }
}
