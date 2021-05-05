using System;
using System.Collections.Generic;

namespace GitHubExplorer.Menu {

    public class MenuElement {
        public string title { get; }
        public string keyIndex { get; }
        public Uri url { get; }
        public int chooseIndex { get; }

        public MenuElement(string keyIndex,string title,Uri url,int chooseIndex) {
            this.title = title;
            this.keyIndex = keyIndex;
            this.url = url;
            this.chooseIndex = chooseIndex;
        }

        public MenuElement() {
            
        }
        public Dictionary<string, MenuElement> DefaultElements() {
            var temp = new Dictionary<string, MenuElement>();
            temp.Add("p", new MenuElement("p","My Profile", new Uri("https://api.github.com/user"), 0));
            temp.Add("r", new MenuElement("r","My Repository", new Uri("https://api.github.com/user/repos"), 1));
            temp.Add("o", new MenuElement("o","My Organization", new Uri("https://api.github.com/user/orgs"), 2));
            temp.Add("q", new MenuElement("q","Quit App", null, -1));
            return temp;
        }

    }

}