using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace FormatSQL
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private int ASSpaceCount = 50;

        private int resultVisibleTime = 1;

        private string msgWait = "クリップボードのSQL文を整形します。";

        private string msgResult = "整形文をコピーしました。";

        private Color colorWait = Colors.Black;

        private Color colorResult = Colors.Red;
        
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sql = this.GetClipBoardText();
                string exSql = this.FormatSQLText(sql);
                this.SetClipBoardText(exSql);
                this.ResultProcess();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string GetClipBoardText()
        {
            return (string)Clipboard.GetData(DataFormats.Text);
        }

        private void SetClipBoardText(string text)
        {
            Clipboard.SetData(DataFormats.Text, text);
        }

        private string FormatSQLText(string sql)
        {
            sql = sql.Replace("[", "");
            sql = sql.Replace("]", "");
            sql = sql.Replace("（", "");
            sql = sql.Replace("(", "");
            sql = sql.Replace("）", "");
            sql = sql.Replace(")", "");
            sql = sql.Replace("／", "");
            sql = this.FullToHalf(sql);
            string pattern = string.Format("^(.{{{0}}}) *AS ", this.ASSpaceCount);
            sql = sql.Replace("   ", "        ");
            sql = sql.Replace(" , ", "      , ");
            sql = sql.Replace("FORM ", "   FORM ");
            sql = Regex.Replace(sql, " AS ", "                                                                 AS ");
            string[] array = sql.Split(new char[]
            {
                '\r',
                '\n'
            });
            StringBuilder newSql = new StringBuilder();
            foreach (string item in array)
            {
                if (!item.Equals(""))
                {
                    newSql.AppendLine(Regex.Replace(item, pattern, "$1AS "));
                }
            }
            return newSql.ToString();
        }

        private string FullToHalf(string strFull)
        {
            return Regex.Replace(strFull, "[Ａ-Ｚ]", (Match p) => (p.Value[0] - 'Ａ' + 'A').ToString());
        }

        private void ResultProcess()
        {
            this.SetMessageStyle(this.msgResult, this.colorResult);
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds((double)this.resultVisibleTime)
            };
            timer.Start();
            timer.Tick += delegate (object s, EventArgs args)
            {
                timer.Stop();
                this.SetMessageStyle(this.msgWait, this.colorWait);
            };
        }

        private void SetMessageStyle(string msg, Color color)
        {
            this.lblMessage.Content = msg;
            this.lblMessage.Foreground = new SolidColorBrush(color);
        }
        
    }
}
