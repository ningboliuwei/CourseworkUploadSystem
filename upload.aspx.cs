using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;

public partial class _Default : Page
{
	private static string pageTitle = "";
	private static ArrayList studentListArray = new ArrayList();
	private static string uploadDirectoryStr = "";

	protected void Page_Load(object sender, EventArgs e)
	{
		MaintainScrollPositionOnPostBack = true;
		EnableViewState = true;
		Title = pageTitle;

		StreamReader sr = null;

		if (Page.IsPostBack != true)
		{
			try
			{
				sr = new StreamReader(Server.MapPath("config.txt"), Encoding.Default);

				string currentLine = "";

				dplCourseworkName.Items.Clear();

				while ((currentLine = sr.ReadLine()) != null)
				{
					if (currentLine == "[Title]")
					{
						lblTitle.Text = sr.ReadLine().Trim();
						pageTitle = lblTitle.Text;
					}
					else if (currentLine == "[Notice Info]")
					{
						lblNotice.Text = sr.ReadLine().Trim();
					}
					else if (currentLine == "[Upload Directory]")
					{
						uploadDirectoryStr = sr.ReadLine().Trim();
					}
					else if (currentLine == "[Student List]")
					{
						string studentRecordStr = "";

						while (studentRecordStr != "[Coursework List]")
						{
							studentRecordStr = sr.ReadLine().Trim();
							string[] studentRecord = new string[2];
							if (studentRecordStr != "[Coursework List]")
							{
								studentRecord = studentRecordStr.Split(',');
								studentListArray.Add(studentRecord);
							}
						}

						dplCourseworkName.Items.Add(sr.ReadLine().Trim());
					}
					else
					{
						dplCourseworkName.Items.Add(currentLine);
					}
				}
			}
			catch (Exception ex)
			{
				lblNotice.Text = "加载本页面出错，请稍候再试。具体错误如下：<br>" + ex.Message;
			}
			finally
			{
				if (sr != null)
				{
					sr.Close();
				}
			}
		}
	}

	protected void btnSubmit_Click(object sender, EventArgs e)
	{
		if (ValidateInput("upload") == true)
		{
			UploadFile();
		}
	}

	private void UploadFile()
	{
		string studentName = txtStudentName.Text.Trim();
		string studentID = txtStudentID.Text.Trim();
		string destFilePath = "";
		string destDirPath = "";

        string path = Server.MapPath("Images\\");
        string filename = "";
        int slashPos = -1;
        int dotPos = -1;
        string shortName = "";
        string fileType = "";

        filename = fupFile.PostedFile.FileName;
        slashPos = filename.LastIndexOf("\\");
        dotPos = filename.LastIndexOf(".");
        shortName = filename.Substring(slashPos + 1);
        fileType = filename.Substring(dotPos + 1);

		StreamWriter sw = null;

		try
		{
			destFilePath = studentID + "+" + studentName + "+" + DateTime.Now.ToString("yyyy'年'MM'月'dd'日' HH'点'mm'分'ss'秒'") +
			               "." + fileType;
			destDirPath = Server.MapPath("") + uploadDirectoryStr + dplCourseworkName.SelectedValue + "\\" + studentID + "+" +
			              studentName + "\\";


			if (Directory.Exists(destDirPath) == false)
			{
				Directory.CreateDirectory(destDirPath);
			}


			fupFile.SaveAs(destDirPath + destFilePath);

			sw = new StreamWriter(Server.MapPath("uploadhistory.txt"), true, Encoding.Default);
			sw.WriteLine(destFilePath + "," + dplCourseworkName.SelectedValue + "," + DateTime.Now.ToString() + "," +
			             Request.ServerVariables.Get("Remote_Addr").ToString());
			sw.Close();

			lblNotice.Text = "上传文件成功，文件名为\"" + destFilePath + "\"。若想进一步确认，请点击\"查看我已经提交的所有文件\"进行查看。";
		}
		catch (Exception ex)
		{
			//lblNotice.Text = "上传文件失败，请稍候重试。";
			lblNotice.Text = ex.Message;
		}
		finally
		{
			if (sw != null)
			{
				sw.Close();
			}
		}
	}


	protected void btnReset_Click(object sender, EventArgs e)
	{
		Response.Redirect("upload.aspx");
	}

	private bool AutoGetStudentName()
	{
		string studentID = txtStudentID.Text.Trim();
		string[] studentRecordArray;
		bool foundStudentID = false;

		for (int i = 0; i < studentListArray.Count; i++)
		{
			studentRecordArray = (string[]) studentListArray[i];
			if (studentRecordArray[0] == studentID)
			{
				txtStudentName.Text = studentRecordArray[1];
				foundStudentID = true;
				break;
			}
		}

		if (foundStudentID == false)
		{
			lblNotice.Text = "此学号未经授权，请检查是否输入了错误的学号，若确认无误，请与任课任课老师联系。";
			txtStudentName.Text = "";
			btnSubmit.Enabled = false;
			lkbViewFiles.Enabled = false;
			return false;
		}
		else
		{
			lblNotice.Text = "注意！请在提交作业前仔细核对学号，姓名与所属的作业名称，错误的输入会导致文件提交到错误的位置。";

			btnSubmit.Enabled = true;
			lkbViewFiles.Enabled = true;
			return true;
		}
	}

	protected void txtStudentID_TextChanged(object sender, EventArgs e)
	{
		AutoGetStudentName();
	}

	protected void lkbViewFiles_Click(object sender, EventArgs e)
	{
		if (ValidateInput("browsefile") == true)
		{
			Session["personalcategoryname"] = txtStudentID.Text.Trim() + "+" + txtStudentName.Text.Trim();
			Session["uploaddirectory"] = Server.MapPath("") + uploadDirectoryStr;
			Response.Redirect("viewfiles.aspx");
		}
	}

	private bool ValidateInput(string type)
	{
		//string studentIDPattern = @"^\d{9}";
		string filePathPattern = @"^(([a-zA-Z]:)|(\\{2}\w+)\$?)(\\(\w[\w ]*.*))+\.(rar|RAR)$";
		bool ifValid = false;

		try
		{
			if (type == "upload")
			{
				if (txtStudentID.Text.Trim() == "")
				{
					lblNotice.Text = "请输入学号";
				}
				else if (fupFile.FileName.Trim() == "")
				{
					lblNotice.Text = "请选择要上传的文件";
				}
				else if (AutoGetStudentName() == false)
				{
					lblNotice.Text = "此学号未经授权，请检查是否输入了错误的学号，若确认无误，请与任课任课老师联系。";
				}
				//else if (Regex.IsMatch(fupFile.PostedFile.FileName, filePathPattern) == false)
				//{
				//    //lblNotice.Text = fupFile.PostedFile.FileName;
				//    lblNotice.Text = "无效的文件路径或文件名，请仔细检查。注意！只能提交RAR格式的文件。";
				//}
				else
				{
					lblNotice.Text = "注意！请在提交作业前仔细核对学号，姓名与所属的作业名称，错误的输入会导致文件提交到错误的位置。";

					ifValid = true;
				}
			}
			else if (type == "browsefile")
			{
				if (txtStudentID.Text.Trim() == "")
				{
					lblNotice.Text = "请输入学号";
				}
				else if (AutoGetStudentName() == false)
				{
					lblNotice.Text = "此学号未经授权，请检查是否输入了错误的学号，若确认无误，请与任课任课老师联系。";
				}
				else
				{
					ifValid = true;
				}
			}
			else
			{
			}

			return ifValid;
		}
		catch (Exception ex)
		{
			throw new Exception(ex.Message);
		}
		
	}
}