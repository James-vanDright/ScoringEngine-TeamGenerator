using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ScoringEngineTeamGenerator
{
	public partial class TeamGen : Form
	{
		/*
		 * The basic idea is this: Upload a template that is populated with a team 1, read through the yaml schema.
		 * the tabs in the ui will then create a team 1, and allow the user to select how many teams, x, they want.
		 * Once selected, x amount of teams/tabs will be generated. Then a save button will be visible, which should splice
		 * all the data together in one neat yaml file.
		 * 
		 * In order to properly serialize/deserialize the YAML, I'm using the YamlDotNot package.
		 * 
		 * So three components are needed:
		 *	- Button to open template
		 *	- Number input
		 *	- Save Button
		 */
		object YAMLObj;
		List<Team> teams = new List<Team>();

		public TeamGen()
		{
			InitializeComponent();
		}

		private void btnTemplate_Click(object sender, EventArgs e)
		{
			DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
			if (result == DialogResult.OK) // Test result.
			{
				string file = openFileDialog1.FileName;
				try
				{
					string text = File.ReadAllText(file);
					var deserializer = new DeserializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build();
					YAMLObj = deserializer.Deserialize(new StringReader(text));

					teamAmount.Visible = true;
					btnTemplate.Visible = false;

					Console.WriteLine( (((YAMLObj as List<object>)[0] as Dictionary<object, object>)["users"] as List<object>)[0] );				
				}
				catch (IOException iox)
				{
					Console.WriteLine("OPEN ISSUE");
					Console.WriteLine(iox.Message);
				}
			}
		}

		private void saveAsToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			DialogResult result = saveFileDialog1.ShowDialog();
			if(result  == DialogResult.OK)
			{
				string file = saveFileDialog1.FileName;
				try
				{
					var serializer = new SerializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build();
					string output = serializer.Serialize(teams);
					using (StreamWriter fs = new StreamWriter(file))
					{
						fs.Write(output);
					}
				}
				catch(IOException iox)
				{
					Console.WriteLine("SAVE ISSUE");
					Console.WriteLine(iox.Message);
				}
				catch(Exception ge)
				{
					Console.WriteLine("SAVE ISSUE");
					Console.WriteLine(ge.Message);
				}
			}
			
			
		}

		private void teamAmount_ValueChanged(object sender, EventArgs e)
		{
			//Revision to this:
			// instead of going through the work of generating each one for each team
			// make one for the first team, then duplicate that FLP to each other team
			// Tricky to pull, considering the overall design method used
			
			for (int i = 0; i < teamAmount.Value; i++)
			{
				GenerateTeamTab(YAMLObj);
				FlowLayoutPanel flp = new FlowLayoutPanel();
				flp.Dock = DockStyle.Fill;
				flp.AutoScroll = true;
				flp.WrapContents = false;
				flp.FlowDirection = FlowDirection.TopDown;

				//Assume that the flp isn't conforming to the size of the newly created tab
				flp.Width = tc_TeamTabs.TabPages[0].Width;
				flp.Height = tc_TeamTabs.TabPages[0].Height;

				flp.Controls.AddRange(GenerateTextLabelDuo("name",teams[i].Name));
				(flp.Controls[1] as TextBox).TextChanged += TeamGen_TeamNameTextChanged;
				flp.Controls.AddRange(GenerateTextLabelDuo("color", teams[i].Color));
				(flp.Controls[3] as TextBox).TextChanged += TeamGen_TeamColorTextChanged;

				//Label for the userDGV
				Label usersLabel = new Label();
				usersLabel.Text = "Team Users";
				flp.Controls.Add(usersLabel);

				flp.Controls.Add(GenerateUserTable(i));

				//label for the serviceDGV
				Label serviceLabel = new Label();
				serviceLabel.Text = "Services";
				flp.Controls.Add(serviceLabel);

				flp.Controls.Add(GenerateServiceTable(i));

				tc_TeamTabs.TabPages[i].Controls.Add(flp);
				
			}
			tc_TeamTabs.Visible = true;
			teamAmount.Visible = false;
			//btnTemplate.Visible = false;
		}

		//This method is what adds new users to the team object, as well as modifies the existing user objects.
		private void UsersDGV_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			DataGridView dgv = (sender as DataGridView);
			//dgv.parent.parent is actually the tabpage container itself. dgv.parent is the flow layout within each tab.
			int teamIndex = tc_TeamTabs.TabPages.IndexOf(dgv.Parent.Parent as TabPage);
			Team team = teams[teamIndex];
			//The blank row at the bottom counts towards rowcount, so off by one needs to be fixed
			if (dgv.Rows.Count-1 > team.users.Count)
				team.users.Add(new User());

			//figuring out which modified field needs to be changed
			if (e.ColumnIndex.Equals(0))
				team.users[e.RowIndex].Username = dgv[e.ColumnIndex, e.RowIndex].Value.ToString();
			else
				team.users[e.RowIndex].Password = dgv[e.ColumnIndex, e.RowIndex].Value.ToString();

		}

		//This is a very important function for future reference
		//What it does is as follows: When the cell is done being changed, reflect and update those changes to all
			//of the team objects.
		//Logically, teams need the same services/environments/matchings/properties
		//While specifics like hosts and users are team specific
		//A line will have to be drawn somewhere, on whether something should be asynchronously or synchronously managed.
		private void EnvDGV_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			DataGridView envDGV = (sender as DataGridView);
			FlowLayoutPanel parentContainer = envDGV.Parent as FlowLayoutPanel;
			ComboBox envCmbox = parentContainer.Controls[2].Controls[0] as ComboBox;
			DataGridView serviceDGV = parentContainer.Controls[0] as DataGridView;

			int serviceIndex = serviceDGV.SelectedCells[0].RowIndex;
			int envCombIndex = envCmbox.SelectedIndex;

			int propCount = teams[0].services[serviceIndex].environments.matchingContent[envCombIndex].properties.Count;

			for(int i = 0; i < teams.Count; i++)
			{
				if(envDGV.Rows.Count - 1 > propCount)
				{
					teams[i].services[serviceIndex].environments.matchingContent[envCombIndex].properties.Add(new Property());
				}

				//figuring out which modified field needs to be changed
				if (e.ColumnIndex.Equals(0))
					teams[i].services[serviceIndex].environments.matchingContent[envCombIndex].properties[e.RowIndex].name = envDGV[e.ColumnIndex, e.RowIndex].Value.ToString();
				else
					teams[i].services[serviceIndex].environments.matchingContent[envCombIndex].properties[e.RowIndex].value = envDGV[e.ColumnIndex, e.RowIndex].Value.ToString();
			}

		}

		private void AcctDGV_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			DataGridView acctDGV = (sender as DataGridView);
			FlowLayoutPanel parentContainer = acctDGV.Parent as FlowLayoutPanel;
			DataGridView serviceDGV = parentContainer.Controls[0] as DataGridView;

			int serviceIndex = serviceDGV.SelectedCells[0].RowIndex;

			int acctCount = teams[0].services[serviceIndex].accounts.Count;

			for(int i = 0; i < teams.Count; i++)
			{
				if(acctDGV.Rows.Count - 1 > acctCount)
				{
					teams[i].services[serviceIndex].accounts.Add(new User());
				}

				if (e.ColumnIndex.Equals(0))
				{
					teams[i].services[serviceIndex].accounts[e.RowIndex].Username = acctDGV[0, e.RowIndex].Value.ToString();
				}
				else
				{
					teams[i].services[serviceIndex].accounts[e.RowIndex].Password = acctDGV[1, e.RowIndex].Value.ToString();
				}

			}
		}

		private void ServiceDGV_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			DataGridView servDGV = (sender as DataGridView);
			FlowLayoutPanel parentContainer = servDGV.Parent as FlowLayoutPanel;

			int serviceIndex = servDGV.SelectedCells[0].RowIndex;

			int serviceCount = teams[0].services.Count;
			
			for(int i = 0;  i < teams.Count; i++)
			{
				if(servDGV.Rows.Count - 1 > serviceCount)
				{
					teams[i].services.Add(new Service());
					
				}
				string value = "";
				if ( !(servDGV[e.ColumnIndex, e.RowIndex].Value is null))
					value = servDGV[e.ColumnIndex, e.RowIndex].Value.ToString();

				switch (e.ColumnIndex)
				{
					case 0:
						teams[i].services[serviceIndex].name = value;

						break;
					case 1:
						teams[i].services[serviceIndex].checkName = value;
						break;
					case 2:
						if(tc_TeamTabs.SelectedIndex == i)
							teams[i].services[serviceIndex].host = value;
						break;
					case 3:
						teams[i].services[serviceIndex].port = value;
						break;
					case 4:
						teams[i].services[serviceIndex].points = value;
						break;
					default:
						break;
				}
				//							Hard coded values...    Main FLP	Second FLP	DataGridView
				DataGridView teamServDGV = tc_TeamTabs.TabPages[i].Controls[0].Controls[7].Controls[0] as DataGridView;
				teamServDGV.Rows.Clear();
				for (int j = 0; j < teams[i].services.Count; j++)
				{
					teamServDGV.Rows.Add(new object[] {
					teams[i].services[j].name,
					teams[i].services[j].checkName,
					 teams[i].services[j].host,
					 teams[i].services[j].port,
					 teams[i].services[j].points
				});
				}
			}

		}

		private void EnvTbox_TextChanged(object sender, EventArgs e)
		{
			TextBox text = sender as TextBox;
			ComboBox comboBox = text.Parent.Controls[0] as ComboBox;

			int envCombIndex = comboBox.SelectedIndex;
			int serviceIndex = (text.Parent.Parent.Controls[0] as DataGridView).SelectedRows[0].Index;

			string newValue = text.Text;

			for(int i = 0; i < teams.Count; i++)
			{
				teams[i].services[serviceIndex].environments.matchingContent[envCombIndex].matchContent = newValue;
			}


		}

		private void AddNew_Click(object sender, EventArgs e)
		{
			Button add = sender as Button;
			int serviceIndex = (add.Parent.Parent.Parent.Controls[0] as DataGridView).SelectedRows[0].Index;

			for (int i = 0; i < teams.Count; i++)
			{
				MatchingContent mc = new MatchingContent();
				mc.matchContent = "Please replace with desired matching content.";
				teams[i].services[serviceIndex].environments.matchingContent.Add(mc);
			}
		}

		private void DeleteCur_Click(object sender, EventArgs e)
		{
			Button del = sender as Button;
			int serviceIndex = (del.Parent.Parent.Parent.Controls[0] as DataGridView).SelectedRows[0].Index;
			int contentIndex = (del.Parent.Parent.Controls[0] as ComboBox).SelectedIndex;


			for(int i = 0; i < teams.Count; i++)
			{
				teams[i].services[serviceIndex].environments.matchingContent.RemoveAt(contentIndex);
			}

		}


		/*
		 * 
		 * 
		 * GENERATE FUNCTIONS
		 *		THESE FUNCTIONS CREATE A VARIETY OF OBJECTS, MOSTLY UI CONTROLS
		 * 
		 * 
		 */
		private void GenerateTeamTab(object YAML)
		{
			Team team = new Team(YAML);
			teams.Add(team);
			TabPage tabPage = new TabPage(team.Name);
			tabPage.Width = tc_TeamTabs.Width;
			tabPage.Height = tc_TeamTabs.Height;


			tc_TeamTabs.TabPages.Add(tabPage);
		
		}

		private Control[] GenerateTextLabelDuo(string key, string val)
		{
			Label label = new Label();
			TextBox text = new TextBox();

			label.AutoSize = true;
			label.Text = key;

			text.Text = val;

			return new Control[] {label, text};

		}

		private Control GenerateUserTable(int teamIndex)
		{
			//USER INPUTS BEGIN HERE
			//
			DataTable userDT = new DataTable();
			userDT.Columns.Add("username", typeof(string));
			userDT.Columns.Add("password", typeof(string));

			//Loop through each team's list of users and add them to their datarows
			for (int j = 0; j < teams[teamIndex].users.Count; j++)
			{
				DataRow row = userDT.NewRow();
				row["username"] = teams[teamIndex].users[j].Username;
				row["password"] = teams[teamIndex].users[j].Password;
				userDT.Rows.Add(row);
			}

			DataGridView usersDGV = new DataGridView();
			usersDGV.CellEndEdit += UsersDGV_CellEndEdit;

			usersDGV.DataSource = userDT;

			return usersDGV;
		}

		private Control GenerateServiceTable(int teamIndex)
		{
			//SERVICE INPUTS BEGIN HERE
			//

			FlowLayoutPanel innerflp = new FlowLayoutPanel();
			innerflp.FlowDirection = FlowDirection.LeftToRight;
			innerflp.AutoSize = true;

			//innerflp.Width = tc_TeamTabs.Width;

			DataGridView serviceDGV = new DataGridView();
			serviceDGV.Columns.Add("name", "Name");
			serviceDGV.Columns.Add("checkName", "Check Name");
			serviceDGV.Columns.Add("host", "Host");
			serviceDGV.Columns.Add("port", "Port");
			serviceDGV.Columns.Add("points", "Points");

			for(int j = 0; j < teams[teamIndex].services.Count; j++)
			{
				serviceDGV.Rows.Add(new object[] {
					teams[teamIndex].services[j].name,
					teams[teamIndex].services[j].checkName,
					 teams[teamIndex].services[j].host,
					 teams[teamIndex].services[j].port,
					 teams[teamIndex].services[j].points
				});
			}
			serviceDGV.SelectionChanged += ServiceDGV_SelectionChanged;
			serviceDGV.CellEndEdit += ServiceDGV_CellEndEdit;

			/*Break down of what to do:
			 *	Select a service
			 *		List box to the right of the dgv shows:
			 *			Accounts
			 *				Selecting this shows a dgv of all accounts for that service, with ability to add more
			 *			Environment
			 *				Selecting this opens another list box showing:
			 *					All matching contents within environment
			 *						Selecting one will open a dgv of all properties related to that matching_content
			 */
			ListBox mainListBox = new ListBox();
			//mainListBox.Visible = false;

			mainListBox.Items.AddRange(new string[] {"Accounts","Environment"});
			mainListBox.SelectedIndexChanged += MainListBox_SelectedIndexChanged;
			
			//THIS WONT WORK FIX IT WITH A DIFFERENT CONTROL
			ComboBox envCmbox = new ComboBox();
			envCmbox.SelectedIndexChanged += EnvCmbox_SelectedIndexChanged;
			//envCmbox.TextChanged += EnvCmbox_Leave;

			TextBox envTbox = new TextBox();
			envTbox.Multiline = true;
			envTbox.Width = envCmbox.Width;
			envTbox.Height = mainListBox.Height - envCmbox.Height;
			envTbox.TextChanged += EnvTbox_TextChanged;

			Button addNew = new Button();
			addNew.Text = "Add";
			addNew.Click += AddNew_Click;
			addNew.Width = envCmbox.Width / 2;
			addNew.Margin = Padding.Empty;

			Button deleteCur = new Button();
			deleteCur.Text = "Delete Selected";
			deleteCur.Click += DeleteCur_Click;
			deleteCur.Width = envCmbox.Width / 2;
			deleteCur.Margin = Padding.Empty;

			FlowLayoutPanel buttonFLP = new FlowLayoutPanel();
			buttonFLP.Controls.AddRange(new Control[] { addNew, deleteCur });
			buttonFLP.Margin = Padding.Empty;
			buttonFLP.Padding = Padding.Empty;
			

			FlowLayoutPanel envFLP = new FlowLayoutPanel();
			envFLP.Controls.AddRange(new Control[] { envCmbox, envTbox, buttonFLP });
			envFLP.FlowDirection = FlowDirection.TopDown;
			envFLP.AutoSize = true;
			envFLP.Visible = false;

			

			DataGridView acctDGV = new DataGridView();
			acctDGV.Columns.Add("Username", "Username");
			acctDGV.Columns.Add("Password", "Password");
			acctDGV.CellEndEdit += AcctDGV_CellEndEdit;
			acctDGV.Visible = false;

			DataGridView envDGV = new DataGridView();
			envDGV.Columns.Add("Property", "Property");
			envDGV.Columns.Add("Value", "Value");
			envDGV.CellEndEdit += EnvDGV_CellEndEdit;
			
			envDGV.Visible = false;

			//innerflp.Height = serviceDGV.Height;
			//Modify the envListBox, it needs to be able to be modified, incase a new matching_content is created.
			//											0			1			2			3			4
			innerflp.Controls.AddRange(new Control[] { serviceDGV, mainListBox, envFLP,  acctDGV, envDGV});

			//The following are purely for seeing the controls spaces
			innerflp.BackColor = Color.Purple;
			buttonFLP.BackColor = Color.Red;
			envFLP.BackColor = Color.Green;
			


			return innerflp;
		}

		private void TeamGen_TeamNameTextChanged(object sender, EventArgs e)
		{
			TextBox textBox = sender as TextBox;

			int teamIndex = tc_TeamTabs.TabPages.IndexOf(textBox.Parent.Parent as TabPage);

			teams[teamIndex].Name = textBox.Text;
			tc_TeamTabs.TabPages[teamIndex].Text = textBox.Text;

		}

		private void TeamGen_TeamColorTextChanged(object sender, EventArgs e)
		{
			TextBox textBox = sender as TextBox;

			int teamIndex = tc_TeamTabs.TabPages.IndexOf(textBox.Parent.Parent as TabPage);

			teams[teamIndex].Color = textBox.Text;
		}

		/*
		 * 
		 * 
		 * SELECTION CHANGED EVENTS
		 * 
		 * 
		 * 
		 */

		private void EnvCmbox_SelectedIndexChanged(object sender, EventArgs e)
		{
			//Get a sense of the local controls
			ComboBox envCmbox = sender as ComboBox;
			FlowLayoutPanel parentContainer = envCmbox.Parent as FlowLayoutPanel;
			DataGridView envDGV = parentContainer.Parent.Controls[4] as DataGridView;
			DataGridView serviceDGV = parentContainer.Parent.Controls[0] as DataGridView;

			//envDGV.Visible = true;

			//same logic as above, where the flp in this case is a child to the main flp within the tab
			int teamIndex = tc_TeamTabs.TabPages.IndexOf(parentContainer.Parent.Parent.Parent as TabPage);
			try
			{
				int serviceIndex = serviceDGV.SelectedCells[0].RowIndex;
				List<Property> properties = teams[teamIndex].services[serviceIndex].environments.matchingContent[envCmbox.SelectedIndex].properties;

				(parentContainer.Controls[1] as TextBox).Text = envCmbox.Text;

				if(envDGV.RowCount > 1)
					envDGV.Rows.Clear();
				for(int i = 0; i < properties.Count; i++)
				{
					envDGV.Rows.Add(properties[i].name, properties[i].value);
				}

			}
			catch(Exception ge)
			{
				Console.WriteLine("Found in EnvCmbox event");
				Console.WriteLine(ge.Message);
			}
			

		}

		private void ServiceDGV_SelectionChanged(object sender, EventArgs e)
		{
			//Get a sense of the local controls
			DataGridView serviceDGV = (sender as DataGridView);
			FlowLayoutPanel parentContainer = serviceDGV.Parent as FlowLayoutPanel;
			ComboBox envCmbox = parentContainer.Controls[2].Controls[0] as ComboBox;
			DataGridView acctDGV = parentContainer.Controls[3] as DataGridView;
			

			//same logic as above, where the flp in this case is a child to the main flp within the tab
			int teamIndex = tc_TeamTabs.TabPages.IndexOf(parentContainer.Parent.Parent as TabPage);
			//Simple names to keep the logic below sensible
			try
			{
				int serviceIndex = serviceDGV.SelectedCells[0].RowIndex;
				Environment serviceEnv = teams[teamIndex].services[serviceIndex].environments;
				List<User> serviceAccts = teams[teamIndex].services[serviceIndex].accounts;

				envCmbox.Items.Clear();
				for (int i = 0; i < serviceEnv.matchingContent.Count; i++)
				{
					envCmbox.Items.Add(serviceEnv.matchingContent[i].matchContent);
				}
				envCmbox.SelectedItem = envCmbox.Items[0];


				acctDGV.Rows.Clear();
				for (int i = 0; i < serviceAccts.Count; i++)
				{
					acctDGV.Rows.Add(serviceAccts[i].Username, serviceAccts[i].Password);
				}
			}
			catch(Exception ge)
			{
				Console.WriteLine("Found in ServiceDGV event");
				Console.WriteLine(ge.Message);
				Console.WriteLine(ge.StackTrace.ToString());
				
			}
		}

		private void MainListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			ListBox mlb = sender as ListBox;
			FlowLayoutPanel parentPanel = mlb.Parent as FlowLayoutPanel;

			if (mlb.SelectedIndex == 0)
			{//Accounts
				parentPanel.Controls[3].Visible = true;
				parentPanel.Controls[2].Visible = false;
				parentPanel.Controls[4].Visible = false;
			}
			else
			{//Environment
				parentPanel.Controls[2].Visible = true;
				parentPanel.Controls[4].Visible = true;

				parentPanel.Controls[3].Visible = false;
				
			}
		}

		
	}
}
