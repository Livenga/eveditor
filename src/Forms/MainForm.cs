using System;
using System.Collections;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


namespace Liven.Forms {
  public class MainForm : Form {
    private ListView lvVariables;
    private Button   btnNew, btnEdit, btnReference, btnDelete;
    private Button   btnUp, btnDown;
    private Button   btnOK, btnCancel;

    private string targetName;
    private EnvironmentVariableTarget envTarget = EnvironmentVariableTarget.Process;


    /// <summary></summary>
#region public MainForm()
    public MainForm() {
      this.InitializeComponent();

      this.Shown += new EventHandler(this.onShownMainForm);

      // ボタンイベント
      this.btnNew.Click       += new EventHandler(this.onClickButtons);
      this.btnEdit.Click      += new EventHandler(this.onClickButtons);
      this.btnReference.Click += new EventHandler(this.onClickButtons);
      this.btnDelete.Click    += new EventHandler(this.onClickButtons);
      this.btnUp.Click        += new EventHandler(this.onClickButtons);
      this.btnDown.Click      += new EventHandler(this.onClickButtons);
      this.btnOK.Click        += new EventHandler(this.onClickButtons);
      this.btnCancel.Click    += new EventHandler(this.onClickButtons);

      // ListView イベント
      this.lvVariables.AfterLabelEdit  += new LabelEditEventHandler(this.onAfterLabelEdit);
      this.lvVariables.DoubleClick     += new EventHandler(this.onDoubleClickVariablesList);
      this.lvVariables.KeyUp           += new KeyEventHandler(this.onKeyUpVariablesList);
      this.lvVariables.Resize          += new EventHandler(this.onResiseVariablesList);
      this.lvVariables.SelectedIndexChanged += new EventHandler(this.onSelectedIndexChangedVariablesList);
    }
#endregion


    // リストの初期化
#region private void InitializeList(string)
    private void InitializeList(string name) {
      string raw_value = Environment.GetEnvironmentVariable(name, this.envTarget);

      if(raw_value == null) {
        return;
      }
      // リストの初期化
      this.lvVariables.Items.Clear();

      // リストの設定
      string[] values = raw_value.Split(new char[]{';'});

      foreach(string v in values) {
        ListViewItem item;

        if(v.Length > 0) {
          item = new ListViewItem(v);
          this.lvVariables.Items.Add(item);
        }
      }

      if(this.lvVariables.Items.Count > 0) {
        this.lvVariables.Items[0].Selected = true;
      }

      this.setListWidth();
    }
#endregion

    private int listWidth = 0;
    //
#region private void setListWidth(string)
    private void setListWidth(string label) {
      int    start_index = 0;
      string target      = null;

      Graphics g;


      if(this.lvVariables.Items.Count == 0) {
        this.listWidth = -2;
      }

      g = this.CreateGraphics();

      if(label != null) {
        start_index = 0;
        target      = label;
      } else {
        start_index = 1;
        target      = this.lvVariables.Items[0].Text;
      }

      for(int i = start_index; i < this.lvVariables.Items.Count; ++i) {
        if(this.lvVariables.Items[i].Text.Length > target.Length) {
          target = this.lvVariables.Items[i].Text;
        }
      }

      SizeF size = g.MeasureString(target, this.Font);

      g.Dispose();
      g = null;

      this.listWidth = (int)size.Width + 10;
    }
#endregion

    //
#region private void setListWidth()
    private void setListWidth() {
      this.setListWidth(null);
    }
#endregion

    //
#region private void PrintEnvironmentVariableForm()
    private void PrintEnvironmentVariableForm() {
      EnvironmentVariableForm ev_form = new EnvironmentVariableForm();

      if(ev_form.ShowDialog() == DialogResult.OK) {
        this.targetName = ev_form.EnvironmentVariableName;
        this.envTarget  = ev_form.EnvironmentVariableTarget;

        this.Text       = "環境変数 \"" + this.targetName + "\" の編集";
        this.InitializeList(this.targetName);
      } else {
        this.Close();
      }

      ev_form.Dispose();
      ev_form = null;
    }
#endregion


    //
    // コールバックイベント
    //

    //
#region private void onShownMainForm(object, EventArgs)
    private void onShownMainForm(object sender, EventArgs e) {
      this.PrintEnvironmentVariableForm();
    }
#endregion

    // ボタン押下時のイベント 各種
#region private void onClickButtons(object, EventArgs)
    private void onClickButtons(object sender, EventArgs e) {
      ListViewItem item;
      Button btn = (Button)sender;

      if(btn.Equals(this.btnNew)) {              // 追加
        item = new ListViewItem("");

        this.lvVariables.Items.Add(item);
        item.BeginEdit();
      }
      else if(btn.Equals(this.btnEdit)) {      // 編集
        if(this.lvVariables.SelectedItems.Count == 1) {
          this.lvVariables.SelectedItems[0].BeginEdit();
        }
      }
      else if(btn.Equals(this.btnReference)) { // 参照
        FolderBrowserDialog dialog;

        dialog = new FolderBrowserDialog();
        dialog.Description =
          "環境変数 \"" + this.targetName + "\" に追加するディレクトリを選択してください.";

        if(dialog.ShowDialog() == DialogResult.OK) {
          // 選択されたディレクトリを一番下に選択された状態で追加.
          item          = new ListViewItem(dialog.SelectedPath);
          item.Selected = true;

          this.lvVariables.Items.Add(item);
        }
      }
      else if(btn.Equals(this.btnDelete)) {    // 削除
        if(this.lvVariables.SelectedItems.Count == 1) {
          ListViewItem lvi_next = null;
          item = this.lvVariables.SelectedItems[0];

          try {
            // 最後のアイテムの場合は, 前のアイテム(-1)を選択, 他は次(+1)
            int idx = ((item.Index + 1) == this.lvVariables.Items.Count) ? -1 : 1;

            // item の次にアイテムが存在しない場合, 例外が発生する.
            lvi_next = this.lvVariables.Items[item.Index + idx];
          } catch(Exception) {
            // 例外が発生した場合は, next に null を代入する.
            lvi_next = null;
          } finally {
            // 次のアイテムが存在する場合, それを選択する
            if(lvi_next != null) {
              lvi_next.Selected = true;
            }
            item.Remove();
          }
        }
      }
      else if(btn.Equals(this.btnUp) || btn.Equals(this.btnDown)) {
        int idx;

        if(this.lvVariables.SelectedItems.Count == 1) {
          item = this.lvVariables.SelectedItems[0];
        } else {
          return;
        }

        if(btn.Equals(this.btnUp)) { // 上
          idx = item.Index - 1;
        } else { // 下
          idx = item.Index + 1;
        }

        this.lvVariables.Items.Remove(item);
        this.lvVariables.Items.Insert(idx, item);
        //Console.WriteLine("Index: {0}, Count: {1}",
        //item.Index, this.lvVariables.Items.Count);
      }
      else if(btn.Equals(this.btnOK) || btn.Equals(this.btnCancel)) { // OK ・ キャンセル
        if(btn.Equals(this.btnOK)) {
            string value;
            StringBuilder sb = new StringBuilder();

            foreach(ListViewItem _item in this.lvVariables.Items) {
            sb.Append(_item.Text);
            sb.Append(";");
            }

            value = sb.ToString();

            sb.Clear();
            sb = null;

            try {
            Environment.SetEnvironmentVariable(
                this.targetName,
                value,
                this.envTarget
                );

            MessageBox.Show("成功しました.", "成功",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            } catch(Exception except) {
              MessageBox.Show("失敗しました.\n" + except.Message, "失敗",
                  MessageBoxButtons.OK, MessageBoxIcon.Error);
              return;
            }
        }

        // 初期状態に遷移
        this.Text = "環境変数の編集";
        this.lvVariables.Items.Clear();
        this.PrintEnvironmentVariableForm();
      }
    }
#endregion

    //
#region private void onAfterLabelEdit(object, LabelEditEventArgs)
    private void onAfterLabelEdit(object sender, LabelEditEventArgs e) {
      ListViewItem item = ((ListView)sender).Items[e.Item];

      if(item.Text.Equals("") && (e.Label == null || e.Label.Equals(""))) {
        item.Remove();
      }

      this.setListWidth(e.Label);
    }
#endregion

    //
#region private void onDoubleClickVariablesList(object, EventArgs)
    private void onDoubleClickVariablesList(object sender, EventArgs e) {
      if(this.lvVariables.SelectedItems.Count == 1) {
        this.lvVariables.SelectedItems[0].BeginEdit();
      } else {
      }
    }
#endregion

    //
#region private void onKeyUpVariablesList(object, KeyEventArgs)
    private void onKeyUpVariablesList(object sender, KeyEventArgs e) {
      ListView lv = (ListView)sender;

      // ListView のアイテムが選択されている状態で F2 キーの押下により
      // ラベルの編集を行う
      if(e.KeyCode == Keys.F2) {
        if(lv.SelectedItems.Count == 1) {
          lv.SelectedItems[0].BeginEdit();
        } else {
        }
      }
    }
#endregion

    //
#region private void onResiseVariablesList(object, EventArgs)
    private void onResiseVariablesList(object sender, EventArgs e) {
      ListView lv = (ListView)sender;

      this.lvVariables.Columns[0].Width =
        (lv.Width > this.listWidth) ? lv.Width : this.listWidth;
    }
#endregion

    // ListView 選択アイテム変更時のイベント
    // 上へ・下へボタンの制御
#region private void onSelectedIndexChangedVariablesList(object, EventArgs)
    private void onSelectedIndexChangedVariablesList(object sender, EventArgs e) {
      ListView lv = (ListView)sender;
      ListViewItem item;

      if(lv.SelectedItems.Count == 0) { return; }

      item = lv.SelectedItems[0];
      if(item.Index == 0) {
        this.btnUp.Enabled   = false;
        this.btnDown.Enabled = true;
      } else if(item.Index == (lv.Items.Count - 1)) {
        this.btnUp.Enabled   = true;
        this.btnDown.Enabled = false;
      } else {
        this.btnUp.Enabled   = true;
        this.btnDown.Enabled = true;
      }
    }
#endregion


    //
    //

    // UI 初期化
#region private void InitializeComponent
    private void InitializeComponent() {
      this.lvVariables  = new ListView();
      this.btnNew       = new Button();
      this.btnEdit      = new Button();
      this.btnReference = new Button();
      this.btnDelete    = new Button();

      this.btnUp        = new Button();
      this.btnDown      = new Button();

      this.btnOK        = new Button();
      this.btnCancel    = new Button();

      this.SuspendLayout();

      this.Controls.Add(this.lvVariables);
      this.Controls.Add(this.btnNew);
      this.Controls.Add(this.btnEdit);
      this.Controls.Add(this.btnReference);
      this.Controls.Add(this.btnDelete);

      this.Controls.Add(this.btnUp);
      this.Controls.Add(this.btnDown);

      this.Controls.Add(this.btnOK);
      this.Controls.Add(this.btnCancel);


      // ListView
      //
      this.lvVariables.Columns.Add("値",
          Screen.PrimaryScreen.Bounds.Width,
          HorizontalAlignment.Left);

      this.lvVariables.Location = new Point(10, 10);
      this.lvVariables.Size     = new Size(170, 210);
      this.lvVariables.Anchor = AnchorStyles.Top | AnchorStyles.Right |
        AnchorStyles.Bottom | AnchorStyles.Left;
      this.lvVariables.HeaderStyle   =  ColumnHeaderStyle.None;
      this.lvVariables.View          = View.Details;
      //this.lvVariables.View          = View.List;
      this.lvVariables.LabelEdit     = true;
      this.lvVariables.FullRowSelect = true;
      this.lvVariables.MultiSelect   = false;
      this.lvVariables.GridLines     = true;

      //
      // ボタン

      // 新規(N)
      this.btnNew.Text     = "新規(&N)";
      this.btnNew.Size     = new Size(90, 25);
      this.btnNew.Anchor   = AnchorStyles.Top | AnchorStyles.Right;
      this.btnNew.Location = new Point(190, 10);

      // 編集(E)
      this.btnEdit.Text     = "編集(&E)";
      this.btnEdit.Size     = new Size(90, 25);
      this.btnEdit.Anchor   = AnchorStyles.Top | AnchorStyles.Right;
      this.btnEdit.Location = new Point(190, 45);

      // 参照(R)
      this.btnReference.Text     = "参照(&R)";
      this.btnReference.Size     = new Size(90, 25);
      this.btnReference.Anchor   = AnchorStyles.Top | AnchorStyles.Right;
      this.btnReference.Location = new Point(190, 80);

      // 削除(D)
      this.btnDelete.Text   = "削除(&D)";
      this.btnDelete.Size   = new Size(90, 25);
      this.btnDelete.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.btnDelete.Location = new Point(190, 115);


      // 上へ(U)
      this.btnUp.Text     = "上へ(&U)";
      this.btnUp.Size     = new Size(90, 25);
      this.btnUp.Anchor   = AnchorStyles.Top | AnchorStyles.Right;
      this.btnUp.Location = new Point(190, this.btnDelete.Bottom + 25);
      
      // 下へ(O)
      this.btnDown.Text     = "下へ(&O)";
      this.btnDown.Size     = new Size(90, 25);
      this.btnDown.Anchor   = AnchorStyles.Top | AnchorStyles.Right;
      this.btnDown.Location = new Point(190, this.btnUp.Bottom + 10);
      Console.WriteLine(this.btnDown.Right);


      // OK
      this.btnOK.Text     = "OK";
      this.btnOK.Anchor   = AnchorStyles.Right | AnchorStyles.Bottom;
      this.btnOK.Size     = new Size(120, 25);
      this.btnOK.Location = new Point(30, 230);

      // キャンセル
      this.btnCancel.Text     = "キャンセル";
      this.btnCancel.Anchor   = AnchorStyles.Right | AnchorStyles.Bottom;
      this.btnCancel.Size     = new Size(120, 25);
      this.btnCancel.Location = new Point(160, 230);


      // Form
      this.Text          = "環境変数の編集";
      this.MinimizeBox   = false;
      this.MaximizeBox   = false;
      this.Size          = new Size(750, 420);
      this.StartPosition = FormStartPosition.CenterScreen;

      this.ResumeLayout();
      this.PerformLayout();
    }
#endregion
  }
}
