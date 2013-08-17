<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmSettings
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Tabs = New System.Windows.Forms.TabControl()
        Me.TabMegj = New System.Windows.Forms.TabPage()
        Me.ChkShowLastIrrev = New System.Windows.Forms.CheckBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.UDShowInterval = New System.Windows.Forms.NumericUpDown()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TabCompPly = New System.Windows.Forms.TabPage()
        Me.ChkIgnoreDD = New System.Windows.Forms.CheckBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.ChkShowEv = New System.Windows.Forms.CheckBox()
        Me.ChkCalcLLNum = New System.Windows.Forms.CheckBox()
        Me.ChkCalcNodeValues = New System.Windows.Forms.CheckBox()
        Me.GrpSkill = New System.Windows.Forms.GroupBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.UDIncTimeLimit = New System.Windows.Forms.NumericUpDown()
        Me.RdExpert = New System.Windows.Forms.RadioButton()
        Me.RdIntermediate = New System.Windows.Forms.RadioButton()
        Me.RdBeginner = New System.Windows.Forms.RadioButton()
        Me.RdNovice = New System.Windows.Forms.RadioButton()
        Me.Tabs.SuspendLayout()
        Me.TabMegj.SuspendLayout()
        CType(Me.UDShowInterval, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabCompPly.SuspendLayout()
        Me.GrpSkill.SuspendLayout()
        CType(Me.UDIncTimeLimit, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Tabs
        '
        Me.Tabs.Controls.Add(Me.TabMegj)
        Me.Tabs.Controls.Add(Me.TabCompPly)
        Me.Tabs.Location = New System.Drawing.Point(12, 12)
        Me.Tabs.Name = "Tabs"
        Me.Tabs.SelectedIndex = 0
        Me.Tabs.Size = New System.Drawing.Size(361, 309)
        Me.Tabs.TabIndex = 0
        '
        'TabMegj
        '
        Me.TabMegj.Controls.Add(Me.ChkShowLastIrrev)
        Me.TabMegj.Controls.Add(Me.Label6)
        Me.TabMegj.Controls.Add(Me.Label2)
        Me.TabMegj.Controls.Add(Me.UDShowInterval)
        Me.TabMegj.Controls.Add(Me.Label1)
        Me.TabMegj.Location = New System.Drawing.Point(4, 22)
        Me.TabMegj.Name = "TabMegj"
        Me.TabMegj.Padding = New System.Windows.Forms.Padding(3)
        Me.TabMegj.Size = New System.Drawing.Size(353, 283)
        Me.TabMegj.TabIndex = 0
        Me.TabMegj.Text = "Appearance"
        Me.TabMegj.UseVisualStyleBackColor = True
        '
        'ChkShowLastIrrev
        '
        Me.ChkShowLastIrrev.AutoSize = True
        Me.ChkShowLastIrrev.Location = New System.Drawing.Point(21, 92)
        Me.ChkShowLastIrrev.Name = "ChkShowLastIrrev"
        Me.ChkShowLastIrrev.Size = New System.Drawing.Size(255, 17)
        Me.ChkShowLastIrrev.TabIndex = 5
        Me.ChkShowLastIrrev.Text = "Show number of irreversible moves left until draw"
        Me.ChkShowLastIrrev.UseVisualStyleBackColor = True
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(18, 39)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(194, 13)
        Me.Label6.TabIndex = 4
        Me.Label6.Text = "(Pressing space shows the mark again.)"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(219, 18)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(20, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "ms"
        '
        'UDShowInterval
        '
        Me.UDShowInterval.Location = New System.Drawing.Point(143, 16)
        Me.UDShowInterval.Maximum = New Decimal(New Integer() {10000, 0, 0, 0})
        Me.UDShowInterval.Name = "UDShowInterval"
        Me.UDShowInterval.Size = New System.Drawing.Size(70, 20)
        Me.UDShowInterval.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(18, 18)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(119, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Time to mark last move:"
        '
        'TabCompPly
        '
        Me.TabCompPly.Controls.Add(Me.ChkIgnoreDD)
        Me.TabCompPly.Controls.Add(Me.Label5)
        Me.TabCompPly.Controls.Add(Me.ChkShowEv)
        Me.TabCompPly.Controls.Add(Me.ChkCalcLLNum)
        Me.TabCompPly.Controls.Add(Me.ChkCalcNodeValues)
        Me.TabCompPly.Controls.Add(Me.GrpSkill)
        Me.TabCompPly.Location = New System.Drawing.Point(4, 22)
        Me.TabCompPly.Name = "TabCompPly"
        Me.TabCompPly.Padding = New System.Windows.Forms.Padding(3)
        Me.TabCompPly.Size = New System.Drawing.Size(353, 283)
        Me.TabCompPly.TabIndex = 1
        Me.TabCompPly.Text = "AI"
        Me.TabCompPly.UseVisualStyleBackColor = True
        '
        'ChkIgnoreDD
        '
        Me.ChkIgnoreDD.Location = New System.Drawing.Point(12, 239)
        Me.ChkIgnoreDD.Name = "ChkIgnoreDD"
        Me.ChkIgnoreDD.Size = New System.Drawing.Size(273, 38)
        Me.ChkIgnoreDD.TabIndex = 9
        Me.ChkIgnoreDD.Text = "&Ignore draw distinguishing info in the databases (strong solution instead of ult" & _
    "ra-strong)"
        Me.ChkIgnoreDD.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.AutoEllipsis = True
        Me.Label5.Location = New System.Drawing.Point(9, 192)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(326, 44)
        Me.Label5.TabIndex = 8
        Me.Label5.Text = "A mélységnél a nagyobb érték nagyobb erősséget jelent. A jelölőnégyzetek közül az" & _
    " első kettő közül csak egy lehet bejelölve, ezek közül a második az erősebb."
        Me.Label5.Visible = False
        '
        'ChkShowEv
        '
        Me.ChkShowEv.AutoSize = True
        Me.ChkShowEv.Location = New System.Drawing.Point(12, 172)
        Me.ChkShowEv.Name = "ChkShowEv"
        Me.ChkShowEv.Size = New System.Drawing.Size(189, 17)
        Me.ChkShowEv.TabIndex = 7
        Me.ChkShowEv.Text = "Show &evaluations of the AI players"
        Me.ChkShowEv.UseVisualStyleBackColor = True
        '
        'ChkCalcLLNum
        '
        Me.ChkCalcLLNum.AutoSize = True
        Me.ChkCalcLLNum.Enabled = False
        Me.ChkCalcLLNum.Location = New System.Drawing.Point(12, 149)
        Me.ChkCalcLLNum.Name = "ChkCalcLLNum"
        Me.ChkCalcLLNum.Size = New System.Drawing.Size(323, 17)
        Me.ChkCalcLLNum.TabIndex = 6
        Me.ChkCalcLLNum.Text = "&Helyzetek kiértékelése lépéslehetőségszám alapján fölrakáskor"
        Me.ChkCalcLLNum.UseVisualStyleBackColor = True
        Me.ChkCalcLLNum.Visible = False
        '
        'ChkCalcNodeValues
        '
        Me.ChkCalcNodeValues.AutoSize = True
        Me.ChkCalcNodeValues.Location = New System.Drawing.Point(12, 126)
        Me.ChkCalcNodeValues.Name = "ChkCalcNodeValues"
        Me.ChkCalcNodeValues.Size = New System.Drawing.Size(248, 17)
        Me.ChkCalcNodeValues.TabIndex = 1
        Me.ChkCalcNodeValues.Text = "Mezők értékeinek &figyelembevétele fölrakáskor"
        Me.ChkCalcNodeValues.UseVisualStyleBackColor = True
        Me.ChkCalcNodeValues.Visible = False
        '
        'GrpSkill
        '
        Me.GrpSkill.Controls.Add(Me.Label4)
        Me.GrpSkill.Controls.Add(Me.UDIncTimeLimit)
        Me.GrpSkill.Controls.Add(Me.RdExpert)
        Me.GrpSkill.Controls.Add(Me.RdIntermediate)
        Me.GrpSkill.Controls.Add(Me.RdBeginner)
        Me.GrpSkill.Controls.Add(Me.RdNovice)
        Me.GrpSkill.Location = New System.Drawing.Point(6, 6)
        Me.GrpSkill.Name = "GrpSkill"
        Me.GrpSkill.Size = New System.Drawing.Size(341, 114)
        Me.GrpSkill.TabIndex = 0
        Me.GrpSkill.TabStop = False
        Me.GrpSkill.Text = "Depths (αβ heuristic AI)"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(201, 90)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(12, 13)
        Me.Label4.TabIndex = 5
        Me.Label4.Text = "s"
        '
        'UDIncTimeLimit
        '
        Me.UDIncTimeLimit.DecimalPlaces = 1
        Me.UDIncTimeLimit.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        Me.UDIncTimeLimit.Location = New System.Drawing.Point(140, 88)
        Me.UDIncTimeLimit.Maximum = New Decimal(New Integer() {10000, 0, 0, 0})
        Me.UDIncTimeLimit.Name = "UDIncTimeLimit"
        Me.UDIncTimeLimit.Size = New System.Drawing.Size(55, 20)
        Me.UDIncTimeLimit.TabIndex = 5
        '
        'RdExpert
        '
        Me.RdExpert.AutoSize = True
        Me.RdExpert.Location = New System.Drawing.Point(6, 88)
        Me.RdExpert.Name = "RdExpert"
        Me.RdExpert.Size = New System.Drawing.Size(128, 17)
        Me.RdExpert.TabIndex = 4
        Me.RdExpert.TabStop = True
        Me.RdExpert.Text = "Minimum thinking &time"
        Me.RdExpert.UseVisualStyleBackColor = True
        '
        'RdIntermediate
        '
        Me.RdIntermediate.AutoSize = True
        Me.RdIntermediate.Enabled = False
        Me.RdIntermediate.Location = New System.Drawing.Point(6, 65)
        Me.RdIntermediate.Name = "RdIntermediate"
        Me.RdIntermediate.Size = New System.Drawing.Size(52, 17)
        Me.RdIntermediate.TabIndex = 3
        Me.RdIntermediate.TabStop = True
        Me.RdIntermediate.Text = "&Large"
        Me.RdIntermediate.UseVisualStyleBackColor = True
        Me.RdIntermediate.Visible = False
        '
        'RdBeginner
        '
        Me.RdBeginner.AutoSize = True
        Me.RdBeginner.Enabled = False
        Me.RdBeginner.Location = New System.Drawing.Point(6, 19)
        Me.RdBeginner.Name = "RdBeginner"
        Me.RdBeginner.Size = New System.Drawing.Size(50, 17)
        Me.RdBeginner.TabIndex = 1
        Me.RdBeginner.TabStop = True
        Me.RdBeginner.Text = "&Small"
        Me.RdBeginner.UseVisualStyleBackColor = True
        Me.RdBeginner.Visible = False
        '
        'RdNovice
        '
        Me.RdNovice.AutoSize = True
        Me.RdNovice.Enabled = False
        Me.RdNovice.Location = New System.Drawing.Point(6, 42)
        Me.RdNovice.Name = "RdNovice"
        Me.RdNovice.Size = New System.Drawing.Size(62, 17)
        Me.RdNovice.TabIndex = 2
        Me.RdNovice.TabStop = True
        Me.RdNovice.Text = "&Medium"
        Me.RdNovice.UseVisualStyleBackColor = True
        Me.RdNovice.Visible = False
        '
        'FrmSettings
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(385, 330)
        Me.Controls.Add(Me.Tabs)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.Name = "FrmSettings"
        Me.Text = "Settings"
        Me.Tabs.ResumeLayout(False)
        Me.TabMegj.ResumeLayout(False)
        Me.TabMegj.PerformLayout()
        CType(Me.UDShowInterval, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabCompPly.ResumeLayout(False)
        Me.TabCompPly.PerformLayout()
        Me.GrpSkill.ResumeLayout(False)
        Me.GrpSkill.PerformLayout()
        CType(Me.UDIncTimeLimit, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Tabs As System.Windows.Forms.TabControl
    Friend WithEvents TabMegj As System.Windows.Forms.TabPage
    Friend WithEvents TabCompPly As System.Windows.Forms.TabPage
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents UDShowInterval As System.Windows.Forms.NumericUpDown
    Friend WithEvents GrpSkill As System.Windows.Forms.GroupBox
    Friend WithEvents RdExpert As System.Windows.Forms.RadioButton
    Friend WithEvents RdIntermediate As System.Windows.Forms.RadioButton
    Friend WithEvents RdBeginner As System.Windows.Forms.RadioButton
    Friend WithEvents RdNovice As System.Windows.Forms.RadioButton
    Friend WithEvents ChkCalcNodeValues As System.Windows.Forms.CheckBox
    Friend WithEvents ChkCalcLLNum As System.Windows.Forms.CheckBox
    Friend WithEvents ChkShowEv As System.Windows.Forms.CheckBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents UDIncTimeLimit As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents ChkIgnoreDD As System.Windows.Forms.CheckBox
    Friend WithEvents ChkShowLastIrrev As System.Windows.Forms.CheckBox
End Class
