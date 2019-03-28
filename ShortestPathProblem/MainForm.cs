﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShortestPathProblem {
	public partial class MainForm : Form {
		private EditCityDialog cityEditor = new EditCityDialog();
		private List<City> cities = new List<City>();
		const string SAVE_FILE_NAME = "SavedCities.dat";

		public MainForm() {
			InitializeComponent();
			CalculateDistance(null, null);
		}

		private void MainForm_Load(object sender, EventArgs e) {
			cities = LoadCitiesFromFile(SAVE_FILE_NAME);
			refreshList();
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
			SaveCitiesToFile(SAVE_FILE_NAME, cities);
		}

		private void refreshList() {
			cities.Sort((x, y) => x.Name.CompareTo(y.Name));
			clearLists();
			foreach (City city in cities) {
				addItemToLists(city);
			}
		}

		private void Btn_AddCity_Click(object sender, EventArgs e) {
			City newCity = new City();
			if(cityEditor.Edit(newCity) == DialogResult.OK) {
				cities.Add(newCity);
				refreshList();
			}
		}

		private void Btn_EditCity_Click(object sender, EventArgs e) {
			City selection = getSelectedCity(CityList);
			if(selection != null) {
				DialogResult result = cityEditor.Edit(selection);
				if (result == DialogResult.OK) refreshList();
				else if (result == DialogResult.No) MessageBox.Show("Error loading city.");
			}
		}

		private void Btn_RemoveCity_Click(object sender, EventArgs e) {
			int index;
			if(TryGetSelection(CityList, out index)) {
				cities.RemoveAt(index);
				removeIndexFromLists(index);
			}
		}

		private void CalculateDistance(object sender, EventArgs e) {
			City city1 = getSelectedCity(CityList2);
			City city2 = getSelectedCity(CityList3);
			double d = 0;
			if(city1 != null && city2 != null) d = city1.DistanceTo(city2);
			Label_Distance_KM.Text = d.ToString("N4").PadLeft(11) + " KM";
			Label_Distance_MI.Text = (d / 1.609).ToString("N4").PadLeft(11) + " MI";
		}

		private bool TryGetSelection(ListBox list, out int index) {
			index = list.SelectedIndex;
			return  ((index >= 0) && (index < cities.Count));
		}

		private City getSelectedCity(ListBox list) {
			int index;
			if(TryGetSelection(list, out index)) {
				return cities[index];
			} else {
				return null;
			}
		}

		private void clearLists() {
			CityList.Items.Clear();
			CityList2.Items.Clear();
			CityList3.Items.Clear();
		}

		private void addItemToLists(City city) {
			CityList.Items.Add(city.Name);
			CityList2.Items.Add(city.Name);
			CityList3.Items.Add(city.Name);
		}

		private void removeIndexFromLists(int index) {
			CityList.Items.RemoveAt(index);
			CityList2.Items.RemoveAt(index);
			CityList3.Items.RemoveAt(index);
		}

		private List<City> LoadCitiesFromFile(string filename) {
			List<City> cities = new List<City>();
			if (!File.Exists(filename)) return cities;
			string[] lines = File.ReadAllLines(filename);
			foreach(string str in lines) {
				City city = new City();
				string[] args = str.Split(',');
				if(args.Length != 3) {
					Console.WriteLine("Unexpected number of arguments: Expected 3, got " + args.Length + ".");
					continue;
				}
				if(args[0] == null || args[1] == null || args[2] == null) {
					Console.WriteLine("There was a null argument.");
					continue;
				}
				if(args[0].Length == 0 || args[1].Length == 0 || args[2].Length == 0) {
					Console.WriteLine("There was an empty argument.");
					continue;
				}

				city.Name = args[0];

				decimal temp;
				if(!decimal.TryParse(args[1], out temp)) {
					Console.WriteLine("Could not read latitude.");
					continue;
				}
				city.Latitude = temp;

				if(!decimal.TryParse(args[2], out temp)) {
					Console.WriteLine("Could not read longitude.");
					continue;
				}
				city.Longitude = temp;

				cities.Add(city);
			}
			
			return cities;
		}

		private void SaveCitiesToFile(string filename, List<City> cities) {
			List<string> lines = new List<string>();
			foreach (City city in cities) {
				string str = city.Name;
				str += "," + city.Latitude.ToString();
				str += "," + city.Longitude.ToString();
				lines.Add(str);
			}
			File.WriteAllLines(filename, lines.ToArray());
		}

	}
}
