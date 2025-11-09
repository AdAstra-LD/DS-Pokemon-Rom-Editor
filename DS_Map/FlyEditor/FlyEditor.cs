using DSPRE.Editors.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DSPRE.RomInfo;

namespace DSPRE.Editors
{
    public partial class FlyEditor : Form
    {
        private const uint DPjpOffset = 0xF41D0;
        private const uint DPusOffset = 0xF2224;
        private const uint DPfrOffset = 0xF2264;
        private const uint DPdeOffset = 0xF2234;
        private const uint DPitOffset = 0xF21D8;
        private const uint DPspOffset = 0xF2270;
        private const int DiamondPearlTableSize = 20;
        private const uint HGSSjpOffset = 0xF9632;
        private const uint HGSSusOffset = 0xF9E82;
        private const uint HGSSfrOffset = 0xF9E66;
        private const uint HGSSdeOffset = 0xF9E36;
        private const uint HGSSitOffset = 0xF9DFA;
        private const uint HGSSspOffset = 0xF9E6A;
        private const int HeartGoldSoulSilverTableSize = 30;
        private const uint PTjpOffset = 0xE8E88;
        private const uint PTusOffset = 0xE97B4;
        private const uint PTfrOffset = 0xE983C;
        private const uint PTdeOffset = 0xE980C;
        private const uint PTitOffset = 0xE97D0;
        private const uint PTspOffset = 0xE9848;
        private const int PlatinumTableSize = 20;
        private static GameFamilies GameFamily;
        private static GameLanguages GameLanguage;
        private List<string> Headers;
        private bool isFormClosing = false;
        private bool isValidInput = true;

        private List<FlyTableRowDpPlat> TableDataDpPlat;
        private List<FlyTableRowHgss> TableDataHgss;

        public FlyEditor(GameFamilies gameFamily, GameLanguages gameLanguage, List<string> headers)
        {
            GameFamily = gameFamily;
            GameLanguage = gameLanguage;
            Headers = headers;
            TableDataHgss = new List<FlyTableRowHgss>();
            TableDataDpPlat = new List<FlyTableRowDpPlat>();
            InitializeComponent();
            PopulateColumns();
            BeginPopulateFlyTableData();
        }

        private static uint FlyTableOffset
        {
            get
            {
                switch (GameFamily)
                {
                    case GameFamilies.DP:
                        switch (GameLanguage)
                        {
                            case GameLanguages.Japanese: return DPjpOffset;
                            case GameLanguages.English: return DPusOffset;
                            case GameLanguages.French: return DPfrOffset;
                            case GameLanguages.German: return DPdeOffset;
                            case GameLanguages.Italian: return DPitOffset;
                            case GameLanguages.Spanish: return DPspOffset;
                            default: throw new ArgumentOutOfRangeException(nameof(GameLanguages), "Unknown language for Diamond/Pearl");
                        }

                    case GameFamilies.Plat:
                        switch (GameLanguage)
                        {
                            case GameLanguages.Japanese: return PTjpOffset;
                            case GameLanguages.English: return PTusOffset;
                            case GameLanguages.French: return PTfrOffset;
                            case GameLanguages.German: return PTdeOffset;
                            case GameLanguages.Italian: return PTitOffset;
                            case GameLanguages.Spanish: return PTspOffset;
                            default: throw new ArgumentOutOfRangeException(nameof(GameLanguages), "Unknown language for Platinum");
                        }

                    case GameFamilies.HGSS:
                        switch (GameLanguage)
                        {
                            case GameLanguages.Japanese: return HGSSjpOffset;
                            case GameLanguages.English: return HGSSusOffset;
                            case GameLanguages.French: return HGSSfrOffset;
                            case GameLanguages.German: return HGSSdeOffset;
                            case GameLanguages.Italian: return HGSSitOffset;
                            case GameLanguages.Spanish: return HGSSspOffset;
                            default: throw new ArgumentOutOfRangeException(nameof(GameLanguages), "Unknown language for HG/SS");
                        }

                    default:
                        throw new ArgumentOutOfRangeException(nameof(GameFamily), "Unknown game family/language.");
                }
            }
        }


        private static int TableSize
        {
            get
            {
                switch (GameFamily)
                {
                    case GameFamilies.DP:
                        return DiamondPearlTableSize;

                    case GameFamilies.Plat:
                        return PlatinumTableSize;

                    case GameFamilies.HGSS:
                        return HeartGoldSoulSilverTableSize;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(GameFamily), "Unknown game family");
                }
            }
        }

        public async void BeginPopulateFlyTableData()
        {
            await PopulateFlyTableDataAsync();
            if (GameFamily == GameFamilies.DP || GameFamily == GameFamilies.Plat)
            {
                await PopulateTablesFromDataDpPlatAsync();
            }
            else if (GameFamily == GameFamilies.HGSS)
            {
                await PopulateTablesFromDataHgssAsync();
            }
        }

        public async Task PopulateFlyTableDataAsync()
        {
            TableDataHgss?.Clear();
            TableDataDpPlat?.Clear();

            try
            {
                using (ARM9.Reader reader = new ARM9.Reader(FlyTableOffset))
                {
                    for (int i = 0; i < TableSize; i++)
                    {
                        if (GameFamily == GameFamilies.HGSS)
                        {
                            FlyTableRowHgss row = new FlyTableRowHgss
                            {
                                HeaderIdGameOver = await ReadUInt16Async(reader),
                                LocalX = await ReadByteAsync(reader),
                                LocalY = await ReadByteAsync(reader),
                                HeaderIdFly = await ReadUInt16Async(reader),
                                GlobalX = await ReadUInt16Async(reader),
                                GlobalY = await ReadUInt16Async(reader),
                                HeaderIdUnlockWarp = await ReadUInt16Async(reader),
                                GlobalXUnlock = await ReadUInt16Async(reader),
                                GlobalYUnlock = await ReadUInt16Async(reader),
                                UnlockId = await ReadByteAsync(reader),
                                WarpCondition = await ReadByteAsync(reader)
                            };
                            TableDataHgss.Add(row);
                        }
                        else if (GameFamily == GameFamilies.DP || GameFamily == GameFamilies.Plat)
                        {
                            FlyTableRowDpPlat row = new FlyTableRowDpPlat
                            {
                                HeaderIdGameOver = await ReadUInt16Async(reader),
                                LocalX = await ReadUInt16Async(reader),
                                LocalY = await ReadUInt16Async(reader),
                                HeaderIdFly = await ReadUInt16Async(reader),
                                GlobalX = await ReadUInt16Async(reader),
                                GlobalY = await ReadUInt16Async(reader),
                                IsTeleportPos = await ReadByteAsync(reader),
                                UnlockOnMapEntry = await ReadByteAsync(reader),
                                UnlockId = await ReadUInt16Async(reader)
                            };
                            TableDataDpPlat.Add(row);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while reading the arm9 file: " + ex.Message);
            }
        }

        public async Task PopulateTablesFromDataDpPlatAsync()
        {
            dt_GameOverWarps.Rows.Clear();
            dt_FlyWarps.Rows.Clear();
            dt_UnlockSettings.Rows.Clear();

            foreach (var row in TableDataDpPlat)
            {
                dt_GameOverWarps.Rows.Add(
                    Headers[row.HeaderIdGameOver],
                    row.LocalX,
                    row.LocalY
                );

                dt_FlyWarps.Rows.Add(
                   Headers[row.HeaderIdFly],
                    row.GlobalX,
                    row.GlobalY
                );

                dt_UnlockSettings.Rows.Add(
                    row.IsTeleportPos == 1,
                    row.UnlockOnMapEntry == 1,
                    row.UnlockId
                );
            }

            await Task.CompletedTask;
        }

        public async Task PopulateTablesFromDataHgssAsync()
        {
            dt_GameOverWarps.Rows.Clear();
            dt_FlyWarps.Rows.Clear();
            dt_UnlockSettings.Rows.Clear();

            foreach (var row in TableDataHgss)
            {
                dt_GameOverWarps.Rows.Add(
                    Headers[row.HeaderIdGameOver],
                    row.LocalX,
                    row.LocalY
                );

                dt_FlyWarps.Rows.Add(
                    Headers[row.HeaderIdFly],
                    row.GlobalX,
                    row.GlobalY
                );

                int newRowIndex = dt_UnlockSettings.Rows.Add(
                    Headers[row.HeaderIdUnlockWarp],
                    row.GlobalXUnlock,
                    row.GlobalYUnlock,
                    row.UnlockId
                    );

                DataGridViewRow newRow = dt_UnlockSettings.Rows[newRowIndex];
                DataGridViewComboBoxCell comboBoxCell = (DataGridViewComboBoxCell)newRow.Cells["warpCondition"];

                comboBoxCell.Value = comboBoxCell.Items[row.WarpCondition];
            }

            await Task.CompletedTask;
        }

        private void AddComboBoxColumn(DataGridView dataGridView, string name, string headerText, List<string> dataSource)
        {
            DataGridViewComboBoxColumn comboBoxColumn = new DataGridViewComboBoxColumn
            {
                Name = name,
                HeaderText = headerText,
                DataSource = dataSource, // Bind the List<string> directly
                DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
            };
            dataGridView.Columns.Add(comboBoxColumn);
        }

        private bool AreAllCellsValid(DataGridView dgv)
        {
            foreach (DataGridViewRow row in dgv.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.ErrorText != string.Empty)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void btn_SaveChanges_Click(object sender, EventArgs e)
        {
            bool hasInvalidCells = false;

            foreach (DataGridViewRow row in dt_GameOverWarps.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (!(dt_GameOverWarps.Columns[cell.ColumnIndex] is DataGridViewComboBoxColumn) &&
                        !(dt_GameOverWarps.Columns[cell.ColumnIndex] is DataGridViewCheckBoxColumn))
                    {
                        if (!ValidateCell(cell))
                        {
                            hasInvalidCells = true;
                            break;
                        }
                    }
                }
            }

            foreach (DataGridViewRow row in dt_FlyWarps.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (!(dt_FlyWarps.Columns[cell.ColumnIndex] is DataGridViewComboBoxColumn) &&
                         !(dt_FlyWarps.Columns[cell.ColumnIndex] is DataGridViewCheckBoxColumn))
                    {
                        if (!ValidateCell(cell))
                        {
                            hasInvalidCells = true;
                            break;
                        }
                    }
                }
            }

            foreach (DataGridViewRow row in dt_UnlockSettings.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (!(dt_UnlockSettings.Columns[cell.ColumnIndex] is DataGridViewComboBoxColumn) &&
                             !(dt_UnlockSettings.Columns[cell.ColumnIndex] is DataGridViewCheckBoxColumn))
                    {
                        if (!ValidateCell(cell))
                        {
                            hasInvalidCells = true;
                            break;
                        }
                    }
                }
            }

            if (!hasInvalidCells)
            {
                try
                {
                    WriteFlyTable();
                    MessageBox.Show("Table data updated!", "Success", MessageBoxButtons.OK);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Unable to Save", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Cannot save! Some fields contain invalid data.");
            }
        }
        private void dt_GameOverWarps_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (dt_GameOverWarps.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn)
            {
                var comboBoxCell = dt_GameOverWarps.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewComboBoxCell;
                if (comboBoxCell != null && !comboBoxCell.Items.Contains(e.FormattedValue))
                {
                    e.Cancel = true;
                    MessageBox.Show("Invalid selection. Please choose a valid value from the list.");
                }
            }
        }
        private void dt_GameOverWarps_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dt_GameOverWarps.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn)
            {
                var comboBoxCell = dt_GameOverWarps.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewComboBoxCell;
                if (comboBoxCell != null)
                {
                    // Perform any additional validation or processing here
                }
            }
        }


        private bool IsValidNumericInput(string input)
        {
            return decimal.TryParse(input, out _);
        }

        private void PopulateColumns()
        {
            var trimmedHeaders = new List<string>();
            foreach (var header in Headers)
            {
                string trimmedHeader = header.TrimEnd('\0');
                trimmedHeaders.Add(trimmedHeader);
            }
            Headers = trimmedHeaders;

            dt_GameOverWarps.Columns.Clear();
            dt_FlyWarps.Columns.Clear();
            dt_UnlockSettings.Columns.Clear();

            dt_GameOverWarps.AllowUserToAddRows = false;
            dt_FlyWarps.AllowUserToAddRows = false;
            dt_UnlockSettings.AllowUserToAddRows = false;

            dt_GameOverWarps.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dt_FlyWarps.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dt_UnlockSettings.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            if (GameFamily == GameFamilies.HGSS)
            {
                AddComboBoxColumn(dt_GameOverWarps, "headerIdGameOver", "Header ID (GameOver)", Headers);
                AddComboBoxColumn(dt_FlyWarps, "headerIdFly", "Header ID (Fly)", Headers);
                AddComboBoxColumn(dt_UnlockSettings, "headerIdUnlock", "Header ID (Unlock)", Headers);

                dt_GameOverWarps.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "localX",
                    HeaderText = "Local X",
                    ValueType = typeof(ushort)
                });
                dt_GameOverWarps.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "localY",
                    HeaderText = "Local Y",
                    ValueType = typeof(ushort)
                });

                dt_FlyWarps.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "globalX",
                    HeaderText = "Global X",
                    ValueType = typeof(ushort)
                });
                dt_FlyWarps.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "globalY",
                    HeaderText = "Global Y",
                    ValueType = typeof(ushort)
                });

                dt_UnlockSettings.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "globalXUnlock",
                    HeaderText = "Global X",
                    ValueType = typeof(ushort)
                });
                dt_UnlockSettings.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "globalYUnlock",
                    HeaderText = "Global Y",
                    ValueType = typeof(ushort)
                });
                dt_UnlockSettings.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "unlockId",
                    HeaderText = "Unlock ID",
                    ValueType = typeof(byte)
                });

                DataGridViewComboBoxColumn warpConditionColumn = new DataGridViewComboBoxColumn
                {
                    Name = "warpCondition",
                    HeaderText = "Warp Condition"
                };
                warpConditionColumn.Items.AddRange(0, 1, 2, 3);
                warpConditionColumn.ValueType = typeof(int);
                dt_UnlockSettings.Columns.Add(warpConditionColumn);
            }
            else if (GameFamily == GameFamilies.DP || GameFamily == GameFamilies.Plat)
            {
                AddComboBoxColumn(dt_GameOverWarps, "headerIdGameOver", "Header ID (GameOver)", Headers);
                AddComboBoxColumn(dt_FlyWarps, "headerIdFly", "Header ID (Fly)", Headers);
                dt_GameOverWarps.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "localX",
                    HeaderText = "Local X",
                    ValueType = typeof(ushort)
                });
                dt_GameOverWarps.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "localY",
                    HeaderText = "Local Y",
                    ValueType = typeof(ushort)
                });

                dt_FlyWarps.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "globalX",
                    HeaderText = "Global X",
                    ValueType = typeof(ushort)
                });
                dt_FlyWarps.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "globalY",
                    HeaderText = "Global Y",
                    ValueType = typeof(ushort)
                });

                DataGridViewCheckBoxColumn isTeleportPosColumn = new DataGridViewCheckBoxColumn
                {
                    Name = "isTeleportPos",
                    HeaderText = "Is Teleport Pos"
                };
                dt_UnlockSettings.Columns.Add(isTeleportPosColumn);

                DataGridViewCheckBoxColumn autoUnlockColumn = new DataGridViewCheckBoxColumn
                {
                    Name = "autoUnlock",
                    HeaderText = "Unlock on Map Entry?"
                };
                dt_UnlockSettings.Columns.Add(autoUnlockColumn);

                dt_UnlockSettings.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "unlockId",
                    HeaderText = "Unlock ID",
                    ValueType = typeof(ushort)
                });
            }
        }

        private async Task<byte> ReadByteAsync(BinaryReader reader)
        {
            byte[] buffer = new byte[1];
            await reader.BaseStream.ReadAsync(buffer, 0, 1);
            return buffer[0];
        }

        private async Task<ushort> ReadUInt16Async(BinaryReader reader)
        {
            byte[] buffer = new byte[2];
            await reader.BaseStream.ReadAsync(buffer, 0, 2);
            return BitConverter.ToUInt16(buffer, 0);
        }

        private bool ValidateCell(DataGridViewCell cell)
        {
            string cellValue = cell.EditedFormattedValue.ToString();

            if (string.IsNullOrWhiteSpace(cellValue) || !IsValidNumericInput(cellValue))
            {
                cell.ErrorText = "Only numeric values are allowed";
                return false;
            }
            else
            {
                cell.ErrorText = string.Empty;
                return true;
            }
        }

        private void WriteFlyTable()
        {
            dt_GameOverWarps.EndEdit();
            dt_FlyWarps.EndEdit();
            dt_UnlockSettings.EndEdit();

            using (ARM9.Writer writer = new ARM9.Writer(FlyTableOffset))
            {
                for (int i = 0; i < TableSize; i++)
                {
                    if (GameFamily == GameFamilies.HGSS)
                    {
                        DataGridViewComboBoxCell comboBoxCell = (DataGridViewComboBoxCell)dt_GameOverWarps.Rows[i].Cells[0];
                        ushort gameOverHeaderId = (ushort)comboBoxCell.Items.IndexOf(comboBoxCell.Value);
                        writer.Write(gameOverHeaderId);

                        byte localX = Convert.ToByte(dt_GameOverWarps.Rows[i].Cells[1].Value);
                        writer.Write(localX);

                        byte localY = Convert.ToByte(dt_GameOverWarps.Rows[i].Cells[2].Value);
                        writer.Write(localY);

                        DataGridViewComboBoxCell comboBoxCellFly = (DataGridViewComboBoxCell)dt_FlyWarps.Rows[i].Cells[0];
                        ushort flyHeaderId = (ushort)comboBoxCellFly.Items.IndexOf(comboBoxCellFly.Value);
                        writer.Write(flyHeaderId);

                        ushort globalX = (ushort)dt_FlyWarps.Rows[i].Cells[1].Value;
                        writer.Write(globalX);

                        ushort globalY = (ushort)dt_FlyWarps.Rows[i].Cells[2].Value;
                        writer.Write(globalY);

                        DataGridViewComboBoxCell comboBoxCellUnlock = (DataGridViewComboBoxCell)dt_UnlockSettings.Rows[i].Cells[0];
                        ushort unlockHeaderId = (ushort)comboBoxCellUnlock.Items.IndexOf(comboBoxCellUnlock.Value);
                        writer.Write(unlockHeaderId);

                        ushort unlockGlobalX = (ushort)dt_UnlockSettings.Rows[i].Cells[1].Value;
                        writer.Write(unlockGlobalX);

                        ushort unlockGlobalY = (ushort)dt_UnlockSettings.Rows[i].Cells[2].Value;
                        writer.Write(unlockGlobalY);

                        byte unlockId = Convert.ToByte(dt_UnlockSettings.Rows[i].Cells[3].Value);
                        writer.Write(unlockId);

                        byte warpCondition = Convert.ToByte(dt_UnlockSettings.Rows[i].Cells[4].Value);
                        writer.Write(warpCondition);
                    }
                    else if (GameFamily == GameFamilies.DP || GameFamily == GameFamilies.Plat)
                    {
                        DataGridViewComboBoxCell comboBoxCellGameOver = (DataGridViewComboBoxCell)dt_GameOverWarps.Rows[i].Cells[0];
                        ushort gameOverHeaderId = (ushort)comboBoxCellGameOver.Items.IndexOf(comboBoxCellGameOver.Value);
                        writer.Write(gameOverHeaderId);

                        ushort localX = (ushort)dt_GameOverWarps.Rows[i].Cells[1].Value;
                        writer.Write(localX);

                        ushort localY = (ushort)dt_GameOverWarps.Rows[i].Cells[2].Value;
                        writer.Write(localY);

                        DataGridViewComboBoxCell comboBoxCellFly = (DataGridViewComboBoxCell)dt_FlyWarps.Rows[i].Cells[0];
                        ushort flyHeaderId = (ushort)comboBoxCellFly.Items.IndexOf(comboBoxCellFly.Value);
                        writer.Write(flyHeaderId);

                        ushort globalX = (ushort)dt_FlyWarps.Rows[i].Cells[1].Value;
                        writer.Write(globalX);

                        ushort globalY = (ushort)dt_FlyWarps.Rows[i].Cells[2].Value;
                        writer.Write(globalY);

                        bool isTeleportPos = (bool)dt_UnlockSettings.Rows[i].Cells[0].Value;
                        writer.Write(isTeleportPos ? (byte)1 : (byte)0);

                        bool autoUnlock = (bool)dt_UnlockSettings.Rows[i].Cells[1].Value;
                        writer.Write(autoUnlock ? (byte)1 : (byte)0);

                        ushort unlockId = (ushort)dt_UnlockSettings.Rows[i].Cells[2].Value;
                        writer.Write(unlockId);
                    }
                }
            }
        }
    }
}