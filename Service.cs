using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoringEngineTeamGenerator
{
	class Service
	{
		public string name;
		public string checkName;
		public string host;
		public string port;
		public string points;
		public List<User> accounts = new List<User>();
		public List<MatchContent> environments = new List<MatchContent>();
	}
}
