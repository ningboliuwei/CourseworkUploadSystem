using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

/// <summary>
/// ConfigManager 的摘要说明
/// </summary>
public class ConfigManager
{
	public ConfigManager()
	{
		CourseworkList = new List<CourseworkInfo>();
		StudentList = new List<StudentInfo>();

	}

	/// <summary>
	/// 标题
	/// </summary>
	public string PageTitle { get; set; }

	/// <summary>
	/// 通告
	/// </summary>
	public string NoticeInfo { get; set; }

	/// <summary>
	/// 上传文件夹名
	/// </summary>
	public string UploadDirectory { get; set; }

	/// <summary>
	/// 所有列表
	/// </summary>
	public List<CourseworkInfo> CourseworkList { get; set; }

	/// <summary>
	/// 学生列表
	/// </summary>
	public List<StudentInfo> StudentList { get; set; }

	public void LoadConfig(string configFilePath)
	{
		StreamReader sr = null;

		CourseworkList.Clear();
		StudentList.Clear();

		try
		{
			sr = new StreamReader(configFilePath, Encoding.Default);

			string currentLine = "";

			while ((currentLine = sr.ReadLine()) != null)
			{
				if (currentLine == "[Title]")
				{
					//读取标题并放入内存中
					PageTitle = sr.ReadLine().Trim();
				}
				else if (currentLine == "[Notice Info]")
				{
					//把提示信息放入内存中
					NoticeInfo = sr.ReadLine().Trim();
				}
				else if (currentLine == "[Upload Directory]")
				{
					//把上传文件夹放入内存中
					UploadDirectory = sr.ReadLine().Trim();
				}
				else if (currentLine == "[Student List]")
				{
					string studentRecordStr = "";

					while (studentRecordStr != "[Coursework List]")
					{
						studentRecordStr = sr.ReadLine().Trim();

						if (studentRecordStr != "[Coursework List]")
						{
							StudentList.Add(new StudentInfo { Sno = studentRecordStr.Split(',')[0], Name = studentRecordStr.Split(',')[1] });
						}
					}

					string[] a = sr.ReadLine().Trim().Split(',');

					CourseworkList.Add(new CourseworkInfo
					{
						ID = a[0],
						Name = a[1],
						PublishTime = Convert.ToDateTime(a[2]),
						DaysBeforeDeadline = Convert.ToInt32(a[3])
					});

				}
				else
				{
					string[] a = currentLine.Split(',');

					CourseworkList.Add(new CourseworkInfo
					{
						ID = a[0],
						Name = a[1],
						PublishTime = Convert.ToDateTime(a[2]),
						DaysBeforeDeadline = Convert.ToInt32(a[3])
					});
				}
			}
		}
		catch (Exception ex)
		{
			throw new Exception(ex.Message);
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