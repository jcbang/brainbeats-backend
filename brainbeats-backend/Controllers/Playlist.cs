using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace brainbeats_backend.Controllers {
  public class Playlist {
		public Playlist(string id) {
			this.id = id;
		}
		public string id {
			get;
			set;
		}

		public string[] beatList {
			get;
			set;
		}
	}
}
