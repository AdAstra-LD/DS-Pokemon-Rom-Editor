using System;
using System.Windows.Forms;

namespace ScintillaNET.Utils {
    internal class SearchManager {

		public Form MainProgram { get; private set; }
		public Scintilla textAreaScintilla { get; private set; }
		public Panel searchPanel { get; private set; }
		public TextBox searchBox { get; private set; }

		public string lastSearched = "";
		public int LastSearchIndex { get; private set; }

		public bool SearchIsOpen = false;
		public SearchManager(Form mainprogram, Scintilla TextArea, TextBox SearchBox, Panel SearchPanel) { 
			this.MainProgram = mainprogram;
			this.textAreaScintilla = TextArea;
			this.searchBox = SearchBox;
			this.searchPanel = SearchPanel;
		}

		public void Find(bool next, bool incremental, string searchThis = null) {
			bool firstTimeResearchingThis = lastSearched != searchThis;

			lastSearched = searchThis == null ? searchBox.Text : searchThis;
			if (lastSearched.Length > 0) {

				if (next) {

					// SEARCH FOR THE NEXT OCCURRENCE

					// Search the document at the last search index
					textAreaScintilla.TargetStart = LastSearchIndex - 1;
					textAreaScintilla.TargetEnd = LastSearchIndex + (lastSearched.Length + 1);
					textAreaScintilla.SearchFlags = SearchFlags.None;

					// Search, and if not found..
					if (!incremental || textAreaScintilla.SearchInTarget(lastSearched) == -1) {

						// Search the document from the caret onwards
						textAreaScintilla.TargetStart = textAreaScintilla.CurrentPosition;
						textAreaScintilla.TargetEnd = textAreaScintilla.TextLength;
						textAreaScintilla.SearchFlags = SearchFlags.None;

						// Search, and if not found..
						if (textAreaScintilla.SearchInTarget(lastSearched) == -1) {

							// Search again from top
							textAreaScintilla.TargetStart = 0;
							textAreaScintilla.TargetEnd = textAreaScintilla.TextLength;

							// Search, and if not found..
							if (textAreaScintilla.SearchInTarget(lastSearched) == -1) {

								// clear selection and exit
								textAreaScintilla.ClearSelections();
								return;
							}
						}

					}

				} else {

					// SEARCH FOR THE PREVIOUS OCCURRENCE

					// Search the document from the beginning to the caret
					textAreaScintilla.TargetStart = textAreaScintilla.CurrentPosition-1;
					textAreaScintilla.TargetEnd = 0;
					textAreaScintilla.SearchFlags = SearchFlags.None;

					// Search, and if not found..
					if (textAreaScintilla.SearchInTarget(lastSearched) == -1) {

						// Search again from the caret onwards
						textAreaScintilla.TargetStart = textAreaScintilla.CurrentPosition;
						textAreaScintilla.TargetEnd = textAreaScintilla.TextLength;

						// Search, and if not found..
						if (textAreaScintilla.SearchInTarget(lastSearched) == -1) {

							// clear selection and exit
							textAreaScintilla.ClearSelections();
							return;
						}
					}

				}

				// Select the occurance
				this.LastSearchIndex = textAreaScintilla.TargetStart;
				textAreaScintilla.SetSelection(textAreaScintilla.TargetEnd, textAreaScintilla.TargetStart);
				textAreaScintilla.ScrollCaret();
			}

			searchBox.Focus();
		}

		public void OpenSearch() {
			if (!SearchIsOpen) {
				SearchIsOpen = true;
				InvokeIfNeeded(MainProgram, delegate () {
					searchPanel.Visible = true;
					searchBox.Text = lastSearched;
					searchBox.Focus();
					searchBox.SelectAll();
				});
			} else {
				InvokeIfNeeded(MainProgram, delegate() {
					searchBox.Focus();
					searchBox.SelectAll();
				});
			}
		}

		public void CloseSearch() {
			if (SearchIsOpen) {
				SearchIsOpen = false;
				InvokeIfNeeded(MainProgram, delegate() {
					searchPanel.Visible = false;
					//CurBrowser.GetBrowser().StopFinding(true);
				});
			}
		}

		public void InvokeIfNeeded(Form MainProgram, Action action) {
			if (MainProgram.InvokeRequired) {
				MainProgram.BeginInvoke(action);
			} else {
				action.Invoke();
			}
		}
	}
}
