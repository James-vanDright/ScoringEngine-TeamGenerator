using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoringEngineTeamGenerator
{
	// This specific group of classes relate to each services environment. 

	class MatchContent
	{
		public string matchingContent;
		public List<Property> properties = new List<Property>();
	}

	class Property
	{
		public string name;
		public string value;

		public Property(string n, string v)
		{
			name = n;
			value = v;
		}

		public Property()
		{

		}

	}
}
