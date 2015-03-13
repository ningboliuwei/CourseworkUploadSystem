<%@ Page Language="C#" AutoEventWireup="true" CodeFile="viewfiles.aspx.cs" Inherits="viewfiles" Debug="true" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>无标题页</title>
</head>
<body background="image/bg8.gif">
    <form id="form1" runat="server">
<div align =center>
	<asp:Label runat="server" ID="lblInfo"></asp:Label></br>
	<asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="upload.aspx">返回提交页面</asp:HyperLink><br />
	<br />
			<asp:GridView ID="gvFiles" runat="server" BackColor="White" BorderColor="#E7E7FF"
			BorderStyle="None" BorderWidth="1px" CellPadding="3" Width="80%" style="font-size: small; font-family: Verdana; background-image: none; background-color: transparent;" EnableModelValidation="True" OnDataBound="gvFiles_DataBound1"  >
			<FooterStyle BackColor="#B5C7DE" ForeColor="#4A3C8C" />
			<RowStyle BackColor="#E7E7FF" ForeColor="#4A3C8C"  HorizontalAlign =center VerticalAlign =Middle/>
			<SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="#F7F7F7" />
			<PagerStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" HorizontalAlign="Right" />
			<HeaderStyle BackColor="#4A3C8C" Font-Bold="True" ForeColor="#F7F7F7" HorizontalAlign="Center" VerticalAlign="Middle" />
			<AlternatingRowStyle BackColor="#F7F7F7" />
				<Columns>
					<asp:BoundField HeaderText="序号" />
				</Columns>
				<EditRowStyle HorizontalAlign="Center" VerticalAlign="Middle" />
		</asp:GridView>
		</div>
    </form>
</body>
</html>
