using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Threading;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace read_textfile_trial
{
    public partial class MainWindow : Window
    {

        #region 定数
        private int resultVisibleTime = 1;  // second
		#endregion

		#region 変数
		private ReadMode _ReadMode = ReadMode.None;

		/// <summary>クラス名 , 変数名 , 型</summary>
		private Dictionary<string, Dictionary<string, string>> _VarList = null;

		#endregion

		#region Enum
		enum ReadMode
		{
			None,
			Table,
			Row
		}	
		#endregion

		#region コンストラクタ
		public MainWindow()
        {
            InitializeComponent();

            // ドラッグアンドドロップ用のイベントハンドラを追加する。
            textbox.AddHandler(TextBox.DragOverEvent, new DragEventHandler(textbox_DragOver), true);
            textbox.AddHandler(TextBox.DropEvent, new DragEventHandler(textbox_Drop), true);
        }
        #endregion

        #region イベント
        private void textbox_DragOver(object sender, DragEventArgs e)
        {
            // マウスポインタを変更する。
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.All;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = false;
        }

        private void textbox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop); // ドロップしたファイル名を全部取得する。
                textbox.Text = filenames[0]; // 最初のファイル名をテキストボックスに表示する。
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            // 文字コードの選択内容を取得する。
            Encoding enc = Encoding.GetEncoding("shift_jis");
            foreach (RadioButton radioCode in panelCode.Children)
            {
                if ((bool)radioCode.IsChecked)
                {
                    enc = Encoding.GetEncoding(radioCode.Tag.ToString());
                    break;
                }
            }

            // テキストファイル読み込み。
            try
            {
				this._ReadMode = ReadMode.None;
                string line = string.Empty;
                using (StreamReader file = new StreamReader(this.textbox.Text))
                {
                    while ((line = file.ReadLine()) != null)
                    {
						// やること
						// １．"Partial Public Class .*DataTable$"を確認	
						// ２．DataSetクラスの"Private Sub InitClass()"を見つける
						// ３．"Me.column* = New Global.System.Data.DataColumn" で宣言している変数名をリストアップする
						// ４．同メソッド内に、以下の分を挿入する
						//     String  = "Me.column{0}.DefaultValue = CType("",String)" : 0=変数名
						//     Numeric = "Me.column{0}.DefaultValue = CType(0D,Decimal)": 0=変数名
						// ５．"Partial Public Class .*Row$"を確認
						// ６．

						// 読取行が、対象のクラス内か判定
						this.SetMode(line, this._ReadMode);
						switch (this._ReadMode)
						{
							case ReadMode.None:
								continue;	// 次行へ

							case ReadMode.Table:

								Match m = Regex.Match(line, "(?<=Partial Public Class )(.*)(?=DataTable)");


								break;

							case ReadMode.Row:



								break;
						}


						if (isDeclare)
                        {
                            isDeclare = Regex.IsMatch(line, "Private .* As Global.System.Data.DataColumn");
                            Match m = Regex.Match(line, "(?<=Private column)(.*)(?= As )");
                            while (m.Success)
                            {

                            }

                        }
						if (!isDeclare)
						{
							isDeclare = Regex.IsMatch(line, "Partial Public Class *DataTable$");
						}
                    }

                }

                // テーブル名・カラム名の取得












                this.ResultProcess();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion



        #region メソッド
		private string SetMode(string line,ReadMode mode)
		{
			string name = string.Empty;
			switch (mode)
			{
				case ReadMode.None:
					if (Regex.IsMatch(line, " *Partial Public Class .*DataTable$"))
					{
						this._ReadMode = ReadMode.Table;
						Match m = Regex.Match(line, "(?<=Partial Public Class )(.*)(?=DataTable)");
						break;
					}
					if (Regex.IsMatch(line," *Partial Public Class .*Row$"))
					{ 
						this._ReadMode = ReadMode.Row;
						break;
					}
					break;

				case ReadMode.Table:
				case ReadMode.Row:
					if (Regex.IsMatch(line , " *End Sub$"))
					{
						this._ReadMode = ReadMode.None;
						break;
					}
					
					break;

			}
		}


        /// <summary>後処理</summary>
        private void ResultProcess()
        {
            this.textblock.Visibility = Visibility.Visible;

            // 1秒後に処理を実行
            DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(this.resultVisibleTime) };
            timer.Start();
            timer.Tick += (s, args) =>
            {
                timer.Stop();

                // 以下に待機後の処理を書く
                this.textblock.Visibility = Visibility.Hidden;
            };
        }
        #endregion
    }
}