
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.IO;
using System.Net.Mime;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace lab9wpf
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string FetchData(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                ContentType type = new ContentType(response.ContentType ?? "text/plain;charset=" + Encoding.UTF8.WebName);
                Encoding encoding = Encoding.GetEncoding(type.CharSet ?? Encoding.UTF8.WebName);

                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream, encoding))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static PageReposne FetchSnippets(int pageNumber, int pageSize, string snippetsType)
        {
            string url = $"https://dirask.com/api/snippets?pageNumber={pageNumber}&pageSize={pageSize}&dataOrder=newest&dataGroup=batches&snippetsType={Uri.EscapeUriString(snippetsType)}";
            string data = FetchData(url);

            return JsonSerializer.Deserialize<PageReposne>(data);
        }

        public static int pageNumber = 1;
        public static int pageSize;
        public static string snippetsType = "text";
        

        public MainWindow()
        {
            InitializeComponent();
            IndexesPerPage.SelectedIndex = 0;
            PageSize();
            var response = FetchSnippets(pageNumber, pageSize, snippetsType);
            List<SnippetResponse> snippets = new List<SnippetResponse>();
            foreach (var s in response.Batches)
                snippets.Add(s);
            grdSnippets.ItemsSource = snippets;
        }

        public void btnTextType_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            snippetsType = (string)b.Tag;
            var response = FetchSnippets(pageNumber, pageSize, (string)b.Tag);
            List<SnippetResponse> snippets = new List<SnippetResponse>();
            foreach (var s in response.Batches)
                snippets.Add(s);

            grdSnippets.ItemsSource = snippets;
        }

        public void btnNumberType_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            pageNumber = int.Parse((string)b.Content);
            var response = FetchSnippets(pageNumber, pageSize, snippetsType);
            List<SnippetResponse> snippets = new List<SnippetResponse>();
            foreach (var s in response.Batches)
                snippets.Add(s);

            grdSnippets.ItemsSource = snippets;
        }

        public void IndexesPerPageChange(object sender, RoutedEventArgs e)
        {
            if (grdSnippets != null)
            {
                PageSize();
                var response = FetchSnippets(pageNumber, pageSize, snippetsType);
                List<SnippetResponse> snippets = new List<SnippetResponse>();
                foreach (var s in response.Batches)
                    snippets.Add(s);

                grdSnippets.ItemsSource = snippets;
            }
        }

        private void PageSize()
        {
            var valueString = IndexesPerPage.SelectedValue.ToString();
            var valueToInt = int.Parse(valueString.Split(":")[1].Trim());

            pageSize = (int)valueToInt;
        }
    }
}
