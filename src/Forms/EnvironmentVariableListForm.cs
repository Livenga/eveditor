using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;


namespace Liven.Forms {
  public class EnvironmentVariableForm : Form {
    public string EnvironmentVariableName {
      get { return this.environmentVariableName; }
    }
    public EnvironmentVariableTarget EnvironmentVariableTarget {
      get { return this.envTarget; }
    }


    private string environmentVariableName = null;
    private EnvironmentVariableTarget envTarget = EnvironmentVariableTarget.Process;


    private Label    lblText;
    private ListView lvEnvironment;
    private Button   btnOK, btnFinish;
    
    private GroupBox    gbxTarget;
    private RadioButton rbProcess, rbUser, rbMachine;


    /// <summary></summary>
#region public EnvironmentVariableForm()
    public EnvironmentVariableForm () {
      this.InitializeComponent();

      // �v�f�ƃC�x���g�̐ݒ�
      this.btnOK.Click     += new EventHandler(this.onClickOK);
      this.btnFinish.Click += new EventHandler(this.onClickFinish);

      //
      this.lvEnvironment.DoubleClick += new EventHandler(this.onDoubleClickEnvironmentList);
      this.lvEnvironment.KeyUp       += new KeyEventHandler(this.onKeyUpEnvironmentList);

      //
      this.rbProcess.CheckedChanged += new EventHandler(this.onChangedCheckedRadioButtons);
      this.rbUser.CheckedChanged    += new EventHandler(this.onChangedCheckedRadioButtons);
      this.rbMachine.CheckedChanged += new EventHandler(this.onChangedCheckedRadioButtons);

      this.SetEnvironmentVariableList(EnvironmentVariableTarget.Process);
    }
#endregion


    //
#region private void SetEnvironmentVariableList(EnvironmentVariableTarget)
    private void SetEnvironmentVariableList(EnvironmentVariableTarget target) {
      IDictionary dict = Environment.GetEnvironmentVariables(target);

      // ListView ������
      this.lvEnvironment.Items.Clear();

      // ListView�̐ݒ�
      foreach(DictionaryEntry entry in dict) {
        ListViewItem lv_item = new ListViewItem(entry.Key.ToString());
        lv_item.SubItems.Add(entry.Value.ToString());

        this.lvEnvironment.Items.Add(lv_item);
      }

      // EnvironmentVariableTarget �����o�������ŕύX
      this.envTarget = target;
    }
#endregion


    //
    //  �R�[���o�b�N�C�x���g
    //

    // OK�{�^���C�x���g
#region private void onClickOK(object, EventArgs)
    private void onClickOK(object sender, EventArgs e) {
      if(this.lvEnvironment.SelectedItems.Count > 0) {
        this.environmentVariableName = this.lvEnvironment.SelectedItems[0].SubItems[0].Text;
        this.DialogResult            = DialogResult.OK;
      } else {
        MessageBox.Show("���ϐ���I�����Ă�������.", "�I��", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
    }
#endregion

    // �I���{�^���C�x���g
#region private void onClickFinish(object, EventArgs)
    private void onClickFinish(object sender, EventArgs e) {
      this.Close();
    }
#endregion

    // ListView �v�f�̃_�u���N���b�N
#region private void onDoubleClickEnvironmentList(object, EventArgs)
    private void onDoubleClickEnvironmentList(object sender, EventArgs e) {
      if(this.lvEnvironment.SelectedItems.Count > 0) {
        this.environmentVariableName = this.lvEnvironment.SelectedItems[0].SubItems[0].Text;
        this.DialogResult = DialogResult.OK;
      }
    }
#endregion

    // ListView �L�[�C�x���g (EnterKey)
#region private void onKeyUpEnvironmentList(object, KeyEventArgs)
    private void onKeyUpEnvironmentList(object sender, KeyEventArgs eKey) {
      if(eKey.KeyCode == Keys.Return && this.lvEnvironment.SelectedItems.Count > 0) {
        this.environmentVariableName = this.lvEnvironment.SelectedItems[0].SubItems[0].Text;
        this.DialogResult = DialogResult.OK;
      }
    }
#endregion

    //
#region private void onChangedCheckedRadioButtons(object, EventArgs)
    private void onChangedCheckedRadioButtons(object sender, EventArgs e) {
      if(this.rbProcess.Checked) { // �v���Z�X
        this.SetEnvironmentVariableList(EnvironmentVariableTarget.Process);
      } else if(this.rbUser.Checked) { // ���[�U
        this.SetEnvironmentVariableList(EnvironmentVariableTarget.User);
      } else if(this.rbMachine.Checked) { // �V�X�e��
        this.SetEnvironmentVariableList(EnvironmentVariableTarget.Machine);
      }
    }
#endregion


    // UI ������
#region private void InitializeComponent
    private void InitializeComponent() {
      this.lblText       = new Label();
      this.lvEnvironment = new ListView();
      this.btnOK         = new Button();
      this.btnFinish     = new Button();
      
      this.gbxTarget = new GroupBox();
      this.rbProcess = new RadioButton();
      this.rbUser    = new RadioButton();
      this.rbMachine = new RadioButton();


      this.SuspendLayout();
      this.gbxTarget.SuspendLayout();

      this.gbxTarget.Controls.Add(this.rbProcess);
      this.gbxTarget.Controls.Add(this.rbUser);
      this.gbxTarget.Controls.Add(this.rbMachine);

      this.Controls.Add(this.gbxTarget);
      this.Controls.Add(this.lblText);
      this.Controls.Add(this.lvEnvironment);
      this.Controls.Add(this.btnOK);
      this.Controls.Add(this.btnFinish);


      //
      this.gbxTarget.Text     = "���ϐ��i�[�ꏊ";
      this.gbxTarget.Size     = new Size(285, 45);
      this.gbxTarget.Location = new Point(10, 10);

      this.rbProcess.Text     = "�v���Z�X(&P)";
      this.rbProcess.Width    = 80;
      this.rbProcess.Checked  = true;
      this.rbProcess.Location = new Point(10, 15);

      this.rbUser.Text     = "���[�U(&U)";
      this.rbUser.Width    = 80;
      this.rbUser.Location = new Point(100, 15);

      this.rbMachine.Text     = "�V�X�e��(&S)";
      this.rbMachine.Width    = 80;
      this.rbMachine.Location = new Point(190, 15);
      
     
      // �e�L�X�g
      this.lblText.Location = new Point(10, 65);
      this.lblText.Text     = "���ϐ��ꗗ";


      // ���ϐ����X�g
      // �w�b�_�̕`��
      this.lvEnvironment.Columns.Add("�ϐ�", -2,  HorizontalAlignment.Left);
      this.lvEnvironment.Columns.Add("�l", -2, HorizontalAlignment.Left);

      this.lvEnvironment.Size          = new Size(575, 225);
      this.lvEnvironment.Location      = new Point(10, this.lblText.Bottom + 5);
      this.lvEnvironment.FullRowSelect = true;
      this.lvEnvironment.MultiSelect   = false;
      this.lvEnvironment.CheckBoxes    = false;
      this.lvEnvironment.GridLines     = true;
      this.lvEnvironment.View          = View.Details;


      // OK�{�^��
      this.btnOK.Text     = "OK";
      this.btnOK.Location = new Point(425, 328);

      // �I��
      this.btnFinish.Text = "�I��";
      this.btnFinish.Location = new Point(510, 328);


      // Form
      this.Text            = "���ϐ����̑I��";
      this.Size            = new Size(600, 385);
      this.MinimizeBox     = false;
      this.MaximizeBox     = false;
      this.ShowInTaskbar   = false;
      this.StartPosition   = FormStartPosition.CenterParent;
      this.FormBorderStyle = FormBorderStyle.FixedSingle;


      this.gbxTarget.ResumeLayout();
      this.gbxTarget.PerformLayout();

      this.ResumeLayout();
      this.PerformLayout();
    }
#endregion
  }
}
