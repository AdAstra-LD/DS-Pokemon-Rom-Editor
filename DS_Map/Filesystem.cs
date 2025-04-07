using System;
using System.IO;

namespace DSPRE {
    public static class Filesystem {
        public static string eventFiles => RomInfo.gameDirs[RomInfo.DirNames.eventFiles].unpackedDir;
        public static string OWSprites => RomInfo.gameDirs[RomInfo.DirNames.OWSprites].unpackedDir;
        public static string mapTextures => RomInfo.gameDirs[RomInfo.DirNames.mapTextures].unpackedDir;
        public static string buildingTextures => RomInfo.gameDirs[RomInfo.DirNames.buildingTextures].unpackedDir;
        public static string dynamicHeaders => RomInfo.gameDirs[RomInfo.DirNames.dynamicHeaders].unpackedDir;
        public static string dynamicHeadersPacked => RomInfo.gameDirs[RomInfo.DirNames.dynamicHeaders].packedDir;
        public static string scripts => RomInfo.gameDirs[RomInfo.DirNames.scripts].unpackedDir;
        public static string maps => RomInfo.gameDirs[RomInfo.DirNames.maps].unpackedDir;
        public static string matrices => RomInfo.gameDirs[RomInfo.DirNames.matrices].unpackedDir;
        public static string buildingConfigFiles => RomInfo.gameDirs[RomInfo.DirNames.buildingConfigFiles].unpackedDir;
        public static string areaData => RomInfo.gameDirs[RomInfo.DirNames.areaData].unpackedDir;
        public static string textArchives => RomInfo.gameDirs[RomInfo.DirNames.textArchives].unpackedDir;
        public static string trainerProperties => RomInfo.gameDirs[RomInfo.DirNames.trainerProperties].unpackedDir;
        public static string trainerParty => RomInfo.gameDirs[RomInfo.DirNames.trainerParty].unpackedDir;
        public static string trainerGraphics => RomInfo.gameDirs[RomInfo.DirNames.trainerGraphics].unpackedDir;
        public static string encounters => RomInfo.gameDirs[RomInfo.DirNames.encounters].unpackedDir;
        public static string headbutt => RomInfo.gameDirs[RomInfo.DirNames.headbutt].unpackedDir;
        public static string safariZone => RomInfo.gameDirs[RomInfo.DirNames.safariZone].unpackedDir;
        public static string monIcons => RomInfo.gameDirs[RomInfo.DirNames.monIcons].unpackedDir;
        public static string synthOverlay => RomInfo.gameDirs[RomInfo.DirNames.synthOverlay].unpackedDir;
        public static string interiorBuildingModels => RomInfo.gameDirs[RomInfo.DirNames.interiorBuildingModels].unpackedDir;
        public static string exteriorBuildingModels => RomInfo.gameDirs[RomInfo.DirNames.exteriorBuildingModels].unpackedDir;

        public static string GetBuildingModelsDirPath(bool interior) {
            if (interior) {
                return interiorBuildingModels;
            } else {
                return exteriorBuildingModels;
            }
        }

        public static string expArmPath => Path.Combine(synthOverlay, PatchToolboxDialog.expandedARMfileID.ToString("D4"));

        public static string GetPath(string path, int id, string format = "D4") {
            return Path.Combine(path, id.ToString(format));
        }

        public static string GetPath(string path, string prefix, int id, string ext, string format = "D4") {
            return Path.Combine(path, prefix + id.ToString(format) + "." + ext);
        }

        static string[] GetBuildingModelFiles(bool interior) {
            return string.IsNullOrWhiteSpace(Filesystem.GetBuildingModelsDirPath(interior)) ? null : Directory.GetFiles(Filesystem.GetBuildingModelsDirPath(interior));
        }

        public static string GetBuildingModelPath(bool interior, int id) {
            return GetPath(Filesystem.GetBuildingModelsDirPath(interior), id);
        }

        public static int GetBuildingCount(bool interior) {
            return GetBuildingModelFiles(interior).Length;
        }

        public static string[] GetAreaDataFiles() {
            return string.IsNullOrWhiteSpace(Filesystem.areaData) ? null : Directory.GetFiles(Filesystem.areaData);
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
            return string.IsNullOrWhiteSpace(Filesystem.mapTextures) ? null : Directory.GetFiles(Filesystem.mapTextures);
        }

        public static string GetMapTexturePath(int id) {
            return GetPath(Filesystem.mapTextures, id);
        }

        public static int GetMapTexturesCount() {
            return GetMapTextureFiles().Length;
        }

        static string[] GetBuildingTextureFiles() {
            return string.IsNullOrWhiteSpace(Filesystem.buildingTextures) ? null : Directory.GetFiles(Filesystem.buildingTextures);
        }

        public static string GetBuildingTexturePath(int id) {
            return GetPath(Filesystem.buildingTextures, id);
        }

        public static int GetBuildingTexturesCount() {
            return GetBuildingTextureFiles().Length;
        }

        static string[] GetMatrixFiles() {
            return string.IsNullOrWhiteSpace(Filesystem.matrices) ? null : Directory.GetFiles(Filesystem.matrices);
        }

        public static string GetMatrixPath(int id) {
            return GetPath(Filesystem.matrices, id);
        }

        public static int GetMatrixCount() {
            return GetMatrixFiles().Length;
        }

        static string[] GetTextArchiveFiles() {
            return string.IsNullOrWhiteSpace(Filesystem.textArchives) ? null : Directory.GetFiles(Filesystem.textArchives);
        }

        public static string GetTextArchivePath(int id) {
            return GetPath(Filesystem.textArchives, id);
        }

        public static int GetTextArchivesCount() {
            return GetTextArchiveFiles().Length;
        }

        static string[] GetMapFiles() {
            return string.IsNullOrWhiteSpace(Filesystem.maps) ? null : Directory.GetFiles(Filesystem.maps);
        }

        public static string GetMapPath(int id) {
            return GetPath(Filesystem.maps, id);
        }

        public static int GetMapCount() {
            return GetMapFiles().Length;
        }

        static string[] GetScriptFiles() {
            return string.IsNullOrWhiteSpace(Filesystem.scripts) ? null : Directory.GetFiles(Filesystem.scripts);
        }

        public static string GetScriptPath(int id) {
            return GetPath(Filesystem.scripts, id);
        }

        public static string GetExportedScriptPath(int id)
        {
            String fileName = String.Format("{0}{1}", id.ToString("D4"), "_script.script");
           
            return Path.Combine(RomInfo.exportedScriptsPath, fileName);
        }

        public static int GetScriptCount() {
            return GetScriptFiles().Length;
        }

        static string[] GetEventFiles() {
            return string.IsNullOrWhiteSpace(Filesystem.eventFiles) ? null : Directory.GetFiles(Filesystem.eventFiles);
        }

        public static string GetEventPath(int id) {
            return GetPath(Filesystem.eventFiles, id);
        }

        public static int GetEventFileCount() {
            return GetEventFiles().Length;
        }

        static string[] GetTrainerPropertiesFiles() {
            return string.IsNullOrWhiteSpace(Filesystem.trainerProperties) ? null : Directory.GetFiles(Filesystem.trainerProperties);
        }

        public static string GetTrainerPropertiesPath(int id) {
            return GetPath(Filesystem.trainerProperties, id);
        }

        public static int GetTrainerPropertiesCount() {
            return GetTrainerPropertiesFiles().Length;
        }

        static string[] GetDynamicHeaderFiles() {
            return string.IsNullOrWhiteSpace(Filesystem.dynamicHeaders) ? null : Directory.GetFiles(Filesystem.dynamicHeaders);
        }

        public static string GetDynamicHeaderPath(int id) {
            return GetPath(Filesystem.dynamicHeaders, id);
        }

        public static int GetDynamicHeadersCount() {
            return GetDynamicHeaderFiles().Length;
        }

        static string[] GetEncounterFiles() {
            return string.IsNullOrWhiteSpace(Filesystem.encounters) ? null : Directory.GetFiles(Filesystem.encounters);
        }

        public static string GetEncounterPath(int id) {
            return GetPath(Filesystem.encounters, id);
        }

        public static int GetEncountersCount() {
            return GetEncounterFiles().Length;
        }

        static string[] GetSafariZoneFiles() {
            return string.IsNullOrWhiteSpace(Filesystem.safariZone) ? null : Directory.GetFiles(Filesystem.safariZone);
        }

        public static string GetSafariZonePath(int id) {
            return GetPath(Filesystem.safariZone, id);
        }

        public static int GetSafariZoneCount() {
            return GetSafariZoneFiles().Length;
        }

        static string[] GetHeadbuttFiles() {
            return string.IsNullOrWhiteSpace(Filesystem.headbutt) ? null : Directory.GetFiles(Filesystem.headbutt);
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
