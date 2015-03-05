<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="upload.aspx.cs" Inherits="_Default"   Debug="true" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	<title>无标题页</title>
<script language="javascript" type="text/javascript">
</script>
</head>
<body background="image/bg8.gif">
	<form id="form1" runat="server">
		<div style="text-align: center; font-size: small; color: black; font-family: Verdana; background-attachment: fixed; background-image: url(image/bg8.gif);" >
				<table style="border-right: maroon thin solid; border-top: maroon thin solid; border-left: maroon thin solid; border-bottom: maroon thin solid; background-attachment: fixed; height: 90%; font-size: 14px;" id="TABLE1"  border = "1" align="center" width="700" cellspacing="0" >
					<tr>
						<td colspan="2" style="height: 100px; text-align: center;"  valign="middle" width="100%" >
							<asp:Label ID="lblTitle" runat="server" style="font-weight: bold; font-size: x-large; color: red; font-family: Verdana" Width="100%" Height="100%"></asp:Label></td>
					</tr>
					<tr>
						<td colspan="2" style="text-align: center;" valign="middle" width="100%" >
							<asp:Label ID="lblNotice" runat="server" style="font-size: medium; color: blue; font-family: Verdana; font-weight: bold;" Height="100%" Width="100%"></asp:Label></td>
					</tr>
					<tr>
						<td c="" style="width: 267px; text-align: center">
							&nbsp;学号（输入学号后按回车，姓名即会自动出现）</td>
						<td style="width: 100px; text-align: justify;">
							<asp:TextBox ID="txtStudentID" runat="server" AutoPostBack="True" MaxLength="9" OnTextChanged="txtStudentID_TextChanged" TabIndex="1"></asp:TextBox></td>
					</tr>
					<tr>
						<td style="width: 267px; text-align: center">
							姓名（无需输入）</td>
						<td style="width: 100px; text-align: justify">
							<asp:TextBox ID="txtStudentName" runat="server" MaxLength="4" ReadOnly="True" TabIndex="2"></asp:TextBox></td>
					</tr>
					<tr>
						<td style="width: 267px; text-align: center">
							作业名称（请选择要提交的文件所属的作业名称）</td>
						<td style="width: 100px; text-align: left">
							<asp:DropDownList ID="dplCourseworkName" runat="server" Width="390px" TabIndex="3" AutoPostBack="True" OnSelectedIndexChanged="dplCourseworkName_SelectedIndexChanged">
							</asp:DropDownList></td>
					</tr>
					<tr>
						<td style="width: 267px; text-align: center;" >
							&nbsp;按“浏览”选择要上传的文件（支持任意格式的文件，多个文件或文件夹请打包为RAR格式）</td>
						<td style="width: 100px; text-align: left;">
							<asp:FileUpload ID="fupFile" runat="server" Width="200px" TabIndex="4" />&nbsp;<asp:Label ID="lblUploadInfo" runat="server" Text=""></asp:Label>
						</td>
					</tr>
					<tr>
						<td colspan="2" >
							&nbsp;<asp:Button ID="btnSubmit" runat="server" Height="32px" Text="提交" Width="72px" OnClick="btnSubmit_Click" CausesValidation="False" Enabled="False" />
							<asp:Button ID="btnReset" runat="server" Height="32px" Text="重置" Width="72px" OnClick="btnReset_Click" CausesValidation="False" />
							</td>
					</tr>
					<tr>
						<td colspan="2">
							<asp:LinkButton ID="lkbViewFiles" runat="server" CausesValidation="False" OnClick="lkbViewFiles_Click" Enabled="False" >查看我已经提交的所有文件</asp:LinkButton>（只需输入正确的学号即可）</td>
					</tr>
				</table>
			</div>
		
	</form>
</body>
</html>
