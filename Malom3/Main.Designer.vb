<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmMain
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmMain))
        Me.MenuStrip = New System.Windows.Forms.MenuStrip()
        Me.MnuJáték = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuNew = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuUndo = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuRedo = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuSetupMode = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuNet = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuCopyHistory = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuCopyMoveList = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuCopy = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuTikzCopy = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuPaste = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuExit = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuJátékosok = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuSwitchSides = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuPl1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuPly1Human = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuPly1Computer = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuPly1Perfect = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuPly1Combined = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuPl2 = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuPly2Human = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuPly2Computer = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuPly2Perfect = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuPly2Combined = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuSettings = New System.Windows.Forms.ToolStripMenuItem()
        Me.AdvisorToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MnuHelp = New System.Windows.Forms.ToolStripMenuItem()
        Me.ManualToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.WebsiteToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.StatusStrip = New System.Windows.Forms.StatusStrip()
        Me.LblKov = New System.Windows.Forms.ToolStripStatusLabel()
        Me.LblSetnum = New System.Windows.Forms.ToolStripStatusLabel()
        Me.LblCalcDepths = New System.Windows.Forms.ToolStripStatusLabel()
        Me.LblTime = New System.Windows.Forms.ToolStripStatusLabel()
        Me.LblEv = New System.Windows.Forms.ToolStripStatusLabel()
        Me.LblSpeed = New System.Windows.Forms.ToolStripStatusLabel()
        Me.LblAligner = New System.Windows.Forms.ToolStripStatusLabel()
        Me.LblPerfEval = New System.Windows.Forms.ToolStripStatusLabel()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.LblLastIrrev = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.MenuStrip.SuspendLayout()
        Me.StatusStrip.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'MenuStrip
        '
        Me.MenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MnuJáték, Me.MnuJátékosok, Me.MnuSettings, Me.AdvisorToolStripMenuItem, Me.MnuHelp})
        Me.MenuStrip.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip.Name = "MenuStrip"
        Me.MenuStrip.Size = New System.Drawing.Size(427, 24)
        Me.MenuStrip.TabIndex = 0
        Me.MenuStrip.Text = "MenuStrip1"
        '
        'MnuJáték
        '
        Me.MnuJáték.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MnuNew, Me.MnuUndo, Me.MnuRedo, Me.MnuSetupMode, Me.MnuNet, Me.MnuCopyHistory, Me.MnuCopyMoveList, Me.MnuCopy, Me.MnuTikzCopy, Me.MnuPaste, Me.MnuExit})
        Me.MnuJáték.Name = "MnuJáték"
        Me.MnuJáték.Size = New System.Drawing.Size(50, 20)
        Me.MnuJáték.Text = "&Game"
        '
        'MnuNew
        '
        Me.MnuNew.Name = "MnuNew"
        Me.MnuNew.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.N), System.Windows.Forms.Keys)
        Me.MnuNew.Size = New System.Drawing.Size(207, 22)
        Me.MnuNew.Text = "&New"
        '
        'MnuUndo
        '
        Me.MnuUndo.Name = "MnuUndo"
        Me.MnuUndo.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Z), System.Windows.Forms.Keys)
        Me.MnuUndo.Size = New System.Drawing.Size(207, 22)
        Me.MnuUndo.Text = "&Undo"
        '
        'MnuRedo
        '
        Me.MnuRedo.Name = "MnuRedo"
        Me.MnuRedo.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Y), System.Windows.Forms.Keys)
        Me.MnuRedo.Size = New System.Drawing.Size(207, 22)
        Me.MnuRedo.Text = "&Redo"
        '
        'MnuSetupMode
        '
        Me.MnuSetupMode.Name = "MnuSetupMode"
        Me.MnuSetupMode.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.S), System.Windows.Forms.Keys)
        Me.MnuSetupMode.Size = New System.Drawing.Size(207, 22)
        Me.MnuSetupMode.Text = "Set Up Position"
        '
        'MnuNet
        '
        Me.MnuNet.Name = "MnuNet"
        Me.MnuNet.ShortcutKeys = System.Windows.Forms.Keys.F4
        Me.MnuNet.Size = New System.Drawing.Size(207, 22)
        Me.MnuNet.Text = "&Tcp/ip kapcsolódás"
        Me.MnuNet.Visible = False
        '
        'MnuCopyHistory
        '
        Me.MnuCopyHistory.Name = "MnuCopyHistory"
        Me.MnuCopyHistory.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.H), System.Windows.Forms.Keys)
        Me.MnuCopyHistory.Size = New System.Drawing.Size(207, 22)
        Me.MnuCopyHistory.Text = "Copy &History"
        '
        'MnuCopyMoveList
        '
        Me.MnuCopyMoveList.Enabled = False
        Me.MnuCopyMoveList.Name = "MnuCopyMoveList"
        Me.MnuCopyMoveList.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.L), System.Windows.Forms.Keys)
        Me.MnuCopyMoveList.Size = New System.Drawing.Size(207, 22)
        Me.MnuCopyMoveList.Text = "Copy Move List"
        Me.MnuCopyMoveList.Visible = False
        '
        'MnuCopy
        '
        Me.MnuCopy.Name = "MnuCopy"
        Me.MnuCopy.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.C), System.Windows.Forms.Keys)
        Me.MnuCopy.Size = New System.Drawing.Size(207, 22)
        Me.MnuCopy.Text = "&Copy Game State"
        '
        'MnuTikzCopy
        '
        Me.MnuTikzCopy.Name = "MnuTikzCopy"
        Me.MnuTikzCopy.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Shift) _
            Or System.Windows.Forms.Keys.C), System.Windows.Forms.Keys)
        Me.MnuTikzCopy.Size = New System.Drawing.Size(207, 22)
        Me.MnuTikzCopy.Text = "Copy &Tikz"
        '
        'MnuPaste
        '
        Me.MnuPaste.Name = "MnuPaste"
        Me.MnuPaste.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.V), System.Windows.Forms.Keys)
        Me.MnuPaste.Size = New System.Drawing.Size(207, 22)
        Me.MnuPaste.Text = "&Paste"
        '
        'MnuExit
        '
        Me.MnuExit.Name = "MnuExit"
        Me.MnuExit.Size = New System.Drawing.Size(207, 22)
        Me.MnuExit.Text = "&Exit"
        '
        'MnuJátékosok
        '
        Me.MnuJátékosok.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MnuSwitchSides, Me.MnuPl1, Me.MnuPl2})
        Me.MnuJátékosok.Name = "MnuJátékosok"
        Me.MnuJátékosok.Size = New System.Drawing.Size(56, 20)
        Me.MnuJátékosok.Text = "&Players"
        '
        'MnuSwitchSides
        '
        Me.MnuSwitchSides.Name = "MnuSwitchSides"
        Me.MnuSwitchSides.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.X), System.Windows.Forms.Keys)
        Me.MnuSwitchSides.Size = New System.Drawing.Size(152, 22)
        Me.MnuSwitchSides.Text = "&Swap"
        '
        'MnuPl1
        '
        Me.MnuPl1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MnuPly1Human, Me.MnuPly1Computer, Me.MnuPly1Perfect, Me.MnuPly1Combined})
        Me.MnuPl1.Name = "MnuPl1"
        Me.MnuPl1.Size = New System.Drawing.Size(152, 22)
        Me.MnuPl1.Text = "&First"
        '
        'MnuPly1Human
        '
        Me.MnuPly1Human.Name = "MnuPly1Human"
        Me.MnuPly1Human.ShortcutKeys = CType((System.Windows.Forms.Keys.Alt Or System.Windows.Forms.Keys.D1), System.Windows.Forms.Keys)
        Me.MnuPly1Human.Size = New System.Drawing.Size(193, 22)
        Me.MnuPly1Human.Text = "&Human"
        '
        'MnuPly1Computer
        '
        Me.MnuPly1Computer.Name = "MnuPly1Computer"
        Me.MnuPly1Computer.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.D1), System.Windows.Forms.Keys)
        Me.MnuPly1Computer.Size = New System.Drawing.Size(193, 22)
        Me.MnuPly1Computer.Text = "Heuristi&c (αβ)"
        '
        'MnuPly1Perfect
        '
        Me.MnuPly1Perfect.Name = "MnuPly1Perfect"
        Me.MnuPly1Perfect.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Shift) _
            Or System.Windows.Forms.Keys.D1), System.Windows.Forms.Keys)
        Me.MnuPly1Perfect.Size = New System.Drawing.Size(193, 22)
        Me.MnuPly1Perfect.Text = "&Perfect"
        '
        'MnuPly1Combined
        '
        Me.MnuPly1Combined.Name = "MnuPly1Combined"
        Me.MnuPly1Combined.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Alt) _
            Or System.Windows.Forms.Keys.D1), System.Windows.Forms.Keys)
        Me.MnuPly1Combined.Size = New System.Drawing.Size(193, 22)
        Me.MnuPly1Combined.Text = "Combine&d"
        '
        'MnuPl2
        '
        Me.MnuPl2.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MnuPly2Human, Me.MnuPly2Computer, Me.MnuPly2Perfect, Me.MnuPly2Combined})
        Me.MnuPl2.Name = "MnuPl2"
        Me.MnuPl2.Size = New System.Drawing.Size(152, 22)
        Me.MnuPl2.Text = "&Second"
        '
        'MnuPly2Human
        '
        Me.MnuPly2Human.Name = "MnuPly2Human"
        Me.MnuPly2Human.ShortcutKeys = CType((System.Windows.Forms.Keys.Alt Or System.Windows.Forms.Keys.D2), System.Windows.Forms.Keys)
        Me.MnuPly2Human.Size = New System.Drawing.Size(193, 22)
        Me.MnuPly2Human.Text = "&Human"
        '
        'MnuPly2Computer
        '
        Me.MnuPly2Computer.Name = "MnuPly2Computer"
        Me.MnuPly2Computer.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.D2), System.Windows.Forms.Keys)
        Me.MnuPly2Computer.Size = New System.Drawing.Size(193, 22)
        Me.MnuPly2Computer.Text = "Heuristi&c (αβ)"
        '
        'MnuPly2Perfect
        '
        Me.MnuPly2Perfect.Name = "MnuPly2Perfect"
        Me.MnuPly2Perfect.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Shift) _
            Or System.Windows.Forms.Keys.D2), System.Windows.Forms.Keys)
        Me.MnuPly2Perfect.Size = New System.Drawing.Size(193, 22)
        Me.MnuPly2Perfect.Text = "&Perfect"
        '
        'MnuPly2Combined
        '
        Me.MnuPly2Combined.Name = "MnuPly2Combined"
        Me.MnuPly2Combined.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Alt) _
            Or System.Windows.Forms.Keys.D2), System.Windows.Forms.Keys)
        Me.MnuPly2Combined.Size = New System.Drawing.Size(193, 22)
        Me.MnuPly2Combined.Text = "Combine&d"
        '
        'MnuSettings
        '
        Me.MnuSettings.Name = "MnuSettings"
        Me.MnuSettings.ShortcutKeys = System.Windows.Forms.Keys.F5
        Me.MnuSettings.Size = New System.Drawing.Size(61, 20)
        Me.MnuSettings.Text = "&Settings"
        '
        'AdvisorToolStripMenuItem
        '
        Me.AdvisorToolStripMenuItem.Name = "AdvisorToolStripMenuItem"
        Me.AdvisorToolStripMenuItem.Size = New System.Drawing.Size(59, 20)
        Me.AdvisorToolStripMenuItem.Text = "&Advisor"
        '
        'MnuHelp
        '
        Me.MnuHelp.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ManualToolStripMenuItem, Me.WebsiteToolStripMenuItem, Me.AboutToolStripMenuItem})
        Me.MnuHelp.Name = "MnuHelp"
        Me.MnuHelp.Size = New System.Drawing.Size(44, 20)
        Me.MnuHelp.Text = "&Help"
        '
        'ManualToolStripMenuItem
        '
        Me.ManualToolStripMenuItem.Name = "ManualToolStripMenuItem"
        Me.ManualToolStripMenuItem.Size = New System.Drawing.Size(116, 22)
        Me.ManualToolStripMenuItem.Text = "&Manual"
        '
        'WebsiteToolStripMenuItem
        '
        Me.WebsiteToolStripMenuItem.Name = "WebsiteToolStripMenuItem"
        Me.WebsiteToolStripMenuItem.Size = New System.Drawing.Size(116, 22)
        Me.WebsiteToolStripMenuItem.Text = "&Website"
        '
        'AboutToolStripMenuItem
        '
        Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        Me.AboutToolStripMenuItem.Size = New System.Drawing.Size(116, 22)
        Me.AboutToolStripMenuItem.Text = "&About"
        '
        'StatusStrip
        '
        Me.StatusStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LblKov, Me.LblSetnum, Me.LblCalcDepths, Me.LblTime, Me.LblEv, Me.LblSpeed, Me.LblAligner, Me.LblPerfEval})
        Me.StatusStrip.Location = New System.Drawing.Point(0, 336)
        Me.StatusStrip.Name = "StatusStrip"
        Me.StatusStrip.ShowItemToolTips = True
        Me.StatusStrip.Size = New System.Drawing.Size(427, 22)
        Me.StatusStrip.TabIndex = 2
        Me.StatusStrip.Text = "StatusStrip1"
        '
        'LblKov
        '
        Me.LblKov.BackColor = System.Drawing.SystemColors.Control
        Me.LblKov.Name = "LblKov"
        Me.LblKov.Size = New System.Drawing.Size(43, 17)
        Me.LblKov.Text = "LblKov"
        '
        'LblSetnum
        '
        Me.LblSetnum.BackColor = System.Drawing.SystemColors.Control
        Me.LblSetnum.Name = "LblSetnum"
        Me.LblSetnum.Size = New System.Drawing.Size(64, 17)
        Me.LblSetnum.Text = "LblSetnum"
        '
        'LblCalcDepths
        '
        Me.LblCalcDepths.BackColor = System.Drawing.SystemColors.Control
        Me.LblCalcDepths.Name = "LblCalcDepths"
        Me.LblCalcDepths.Size = New System.Drawing.Size(10, 17)
        Me.LblCalcDepths.Text = " "
        '
        'LblTime
        '
        Me.LblTime.BackColor = System.Drawing.SystemColors.Control
        Me.LblTime.Name = "LblTime"
        Me.LblTime.Size = New System.Drawing.Size(10, 17)
        Me.LblTime.Text = " "
        '
        'LblEv
        '
        Me.LblEv.BackColor = System.Drawing.SystemColors.Control
        Me.LblEv.Name = "LblEv"
        Me.LblEv.Size = New System.Drawing.Size(10, 17)
        Me.LblEv.Text = " "
        Me.LblEv.ToolTipText = "The evaluation of the computer player's position, according to the heuristic AI."
        '
        'LblSpeed
        '
        Me.LblSpeed.BackColor = System.Drawing.SystemColors.Control
        Me.LblSpeed.Name = "LblSpeed"
        Me.LblSpeed.Size = New System.Drawing.Size(10, 17)
        Me.LblSpeed.Text = " "
        '
        'LblAligner
        '
        Me.LblAligner.BackColor = System.Drawing.SystemColors.Control
        Me.LblAligner.Name = "LblAligner"
        Me.LblAligner.Size = New System.Drawing.Size(255, 17)
        Me.LblAligner.Spring = True
        Me.LblAligner.Text = " "
        '
        'LblPerfEval
        '
        Me.LblPerfEval.BackColor = System.Drawing.SystemColors.Control
        Me.LblPerfEval.Name = "LblPerfEval"
        Me.LblPerfEval.Size = New System.Drawing.Size(10, 17)
        Me.LblPerfEval.Text = " "
        Me.LblPerfEval.ToolTipText = "The evaluation of the current game state, according to the database. (See Readme." & _
    "txt for an explanation.)"
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LblLastIrrev})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 314)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(427, 22)
        Me.StatusStrip1.TabIndex = 3
        Me.StatusStrip1.Text = "StatusStrip2"
        '
        'LblLastIrrev
        '
        Me.LblLastIrrev.BackColor = System.Drawing.SystemColors.Control
        Me.LblLastIrrev.Name = "LblLastIrrev"
        Me.LblLastIrrev.Size = New System.Drawing.Size(10, 17)
        Me.LblLastIrrev.Text = " "
        '
        'FrmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(160, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(160, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(427, 358)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.StatusStrip)
        Me.Controls.Add(Me.MenuStrip)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip
        Me.Name = "FrmMain"
        Me.Text = "Malom"
        Me.MenuStrip.ResumeLayout(False)
        Me.MenuStrip.PerformLayout()
        Me.StatusStrip.ResumeLayout(False)
        Me.StatusStrip.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents MenuStrip As System.Windows.Forms.MenuStrip
    Friend WithEvents MnuJáték As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuNew As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuExit As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents StatusStrip As System.Windows.Forms.StatusStrip
    Friend WithEvents LblKov As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents MnuNet As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents LblSetnum As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents MnuJátékosok As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuPl1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuPly1Human As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuPly1Computer As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuPl2 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuPly2Human As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuPly2Computer As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuSwitchSides As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuUndo As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuRedo As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuCopy As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuPaste As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuSettings As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents LblCalcDepths As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents LblTime As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents LblEv As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents MnuHelp As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents LblSpeed As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents MnuPly1Perfect As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuPly2Perfect As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents LblAligner As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents LblPerfEval As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents LblLastIrrev As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents MnuCopyHistory As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuTikzCopy As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuCopyMoveList As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AdvisorToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuPly1Combined As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuPly2Combined As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ManualToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents WebsiteToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AboutToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MnuSetupMode As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip

End Class
