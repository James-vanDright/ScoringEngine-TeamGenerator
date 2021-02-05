using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoringEngineTeamGenerator
{
	class Team
	{
		public string Name;
		public string Color;

		public List<Service> services = new List<Service>();
		public List<User> users = new List<User>();

		public Team(object YAML)
		{
			//There are multiple methods to dynamically deserialize an object, especially one with a few layers of
			// complexity, like the competition.yaml file.
			//The method I'm employing is a more brute-force way. We know the general design of the object, just not the specifics.
			//To simplify the way everything is written out, it is recommended to break each dictionary/list down when possible.

			//This object is a singular team. The [0] is actually the first team in the yaml.
			Dictionary<object, object> rootObject = ((YAML as List<object>)[0] as Dictionary<object, object>);

			Name = rootObject["name"].ToString();
			Color = rootObject["color"].ToString();

			int userCount = (rootObject["users"] as List<object>).Count;
			int serviceCount = (rootObject["services"] as List<object>).Count;

			for (int i = 0; i < userCount; i++)
			{
				//Users in each team are broken down into dictionary objects.
				Dictionary<object, object> userObject = ((rootObject["users"] as List<object>)[i] as Dictionary<object, object>);
				string uname = userObject["username"] as string;
				string upass = userObject["password"] as string;

				users.Add(new User(uname, upass));
			}

			for (int i = 0; i < serviceCount; i++)
			{
				//Same idea applies to services
				Dictionary<object, object> serviceObject = ((rootObject["services"] as List<object>)[i] as Dictionary<object, object>);

				Service tmpService = new Service();

				tmpService.name = serviceObject["name"] as string;
				tmpService.checkName = serviceObject["check_name"] as string;
				tmpService.host = serviceObject["host"] as string;
				tmpService.port = serviceObject["port"] as string;
				tmpService.points = serviceObject["points"] as string;

				if (serviceObject.ContainsKey("accounts"))
				{
					for (int j = 0; j < (serviceObject["accounts"] as List<object>).Count; j++)
					{
						//Using the above user logic, applied to the 
						Dictionary<object, object> userObject = ((serviceObject["accounts"] as List<object>)[j] as Dictionary<object, object>);
						tmpService.accounts.Add(new User(userObject["username"] as string, userObject["password"] as string));

					}
				}
				

				for(int j = 0; j < (serviceObject["environments"] as List<object>).Count; j++)
				{
					Dictionary<object, object> matchingObject = ((serviceObject["environments"] as List<object>)[j] as Dictionary<object, object>);

					MatchingContent mc = new MatchingContent();
					mc.matchContent = matchingObject["matching_content"] as string;
					if (matchingObject.ContainsKey("properties"))
					{
						for (int h = 0; h < (matchingObject["properties"] as List<object>).Count; h++)
						{
							Property property = new Property();
							property.name = ((matchingObject["properties"] as List<object>)[h] as Dictionary<object, object>)["name"] as string;
							property.value = ((matchingObject["properties"] as List<object>)[h] as Dictionary<object, object>)["value"] as string;

							mc.properties.Add(property);
						}
					}
					tmpService.environment.matchingContents.Add(mc);

				}

				services.Add(tmpService);

			}

		}
	}
}
