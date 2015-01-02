﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace SummonersWarRuneBuilder
{

    /// <summary>
    /// Interaction logic for RuneWindow.xaml
    /// </summary>
    public partial class RuneWindow : Window, INotifyPropertyChanged
    {
        
        //Main object declarations
        private Rune _rune;
        private List<String> PropertyList;
        public List<String> TypeList;
        public List<String> GradeList;
        public List<String> UpgradeAmount;
        public List<String> StarList;
        public List<String> LevelList;


        private ComboBox[] secondaryPropertyComboBoxes;
        private ComboBox[] secondaryAmountComboBoxes;
        private ComboBox[] upgradePropertyComboBoxes;
        private ComboBox[] upgradeAmountComboBoxes;
        private TextBlock[] secondaryPropertyFinal;
        private TextBlock[] secondaryAmountFinal;
        
        private ObservableCollection<string> _mainPropertyList;
        public ObservableCollection<string> mainPropertyList
        {
            get
            {
                return _mainPropertyList;
            }
            set
            {
                _mainPropertyList = value;
                NotifyPropertyChanged("mainPropertyList"); // method implemented below
            }
        }

        public ObservableCollection<string> upgradePropertyList;
       
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,new PropertyChangedEventArgs(name));
            }
        }

        private List<String> _mainSelectedPropertyList;


        public event EventHandler<WindowEventArgs> DialogFinished;

        
        //End object declarations
        
        //Main methods

        public RuneWindow(Rune rune)
        {
            _rune = rune;
            InitializeComponent();
            linkWindowObjects();
            createPropertyLists();
            ConsoleLog.Text = _rune.level.ToString();
            initComboBoxes();
            
        }
        
        private void createPropertyLists() 
        {
            PropertyList = Enum.GetNames(typeof(RuneStat.Property)).ToList();
            
            mainPropertyList = new ObservableCollection<String>(PropertyList);
            _mainSelectedPropertyList = new List<String>();
            upgradePropertyList = new ObservableCollection<String>(PropertyList);

            TypeList = Enum.GetNames(typeof(Rune.Type)).ToList();
            GradeList = Enum.GetNames(typeof(Rune.Grade)).ToList();
            UpgradeAmount = new List<String>();
            StarList = Rune.Stars.Select(i => i.ToString()).ToList();
            LevelList = Rune.Levels.Select(i => i.ToString()).ToList();

        }

        private void initComboBoxes()
        {

            bindMainPropertyList();
            bindUpgradePropertyList();
            bindUpgradeAmount();
            bindOtherComboBoxes();

            resetMainPropertyCounts();
            resetUpgradePropertyCounts();

        }

        private void linkWindowObjects() 
        {

            secondaryPropertyComboBoxes = new ComboBox[4] { SecondaryPropertyCombo1, SecondaryPropertyCombo2, SecondaryPropertyCombo3, SecondaryPropertyCombo4 };
            secondaryAmountComboBoxes = new ComboBox[4] { SecondaryAmountCombo1, SecondaryAmountCombo2, SecondaryAmountCombo3, SecondaryAmountCombo4 };
            upgradePropertyComboBoxes = new ComboBox[4] { Upgrade3PropertyCombo, Upgrade6PropertyCombo, Upgrade9PropertyCombo, Upgrade12PropertyCombo };
            upgradeAmountComboBoxes = new ComboBox[4] { Upgrade3AmountCombo, Upgrade6AmountCombo, Upgrade9AmountCombo, Upgrade12AmountCombo };
            secondaryPropertyFinal = new TextBlock[4] { SecondaryPropertyOverall1, SecondaryPropertyOverall2, SecondaryPropertyOverall3, SecondaryPropertyOverall4 };
            secondaryAmountFinal = new TextBlock[4] { SecondaryAmountOverall1, SecondaryAmountOverall2, SecondaryAmountOverall3, SecondaryAmountOverall4 };
            
        }

        private void loadRuneInfo()
        {
           
        }

        public Boolean calculate()
        {
            int level = Int32.Parse((string)LevelCombo.SelectedItem);
            int star = Int32.Parse((string)StarCombo.SelectedItem);
            String type = (string)TypeCombo.SelectedItem;
            Rune.Type runeType = (Rune.Type)Enum.Parse(typeof(Rune.Type), type, true);
            _rune.setProperties(star, runeType);
            _rune.setLevel(level);
            setPrimary();
            setSecondary();
            setUpgrades();
            setInherent();

            _rune.calculate();

            displayPrimary();
            displaySecondary();
            displayInherent();

            return true;
        }

        private Boolean setPrimary()
        {
            RuneStat.Property runeProperty = RuneStat.toProperty((string)PrimaryPropertyCombo.SelectedItem);
            _rune.setPrimaryProperty(runeProperty);
            return true;
        }
        
        private Boolean setInherent()
        {
            if (InnatePropertyCombo.SelectedItem == null || InnateAmountCombo.SelectedItem == null)
            {
                return false;
            }
            RuneStat.Property runeProperty = RuneStat.toProperty((string)InnatePropertyCombo.SelectedItem);
            int runeAmount = 0;
            if (InnateAmountCombo.SelectedIndex != 0)
            {
                runeAmount = (int)InnateAmountCombo.SelectedItem;
            }
            else
            {
                return false;
            }
            _rune.setInherentProperty(new RuneStat(runeProperty, runeAmount));
            return true;
        }

        private Boolean displayPrimary()
        {
            RuneStat stat = _rune.getPrimary();
            PrimaryPropertyOverall.Text = stat.property.ToString();
            PrimaryAmountOverall.Text = _rune.getPrimary().amount.ToString();
            
            return true;
        }

        private Boolean displayInherent()
        {
            RuneStat stat = _rune.getInherent();
            InnatePropertyOverall.Text = stat.property.ToString();
            InnateAmountOverall.Text = stat.amount.ToString();
            return true;
        }

        private Boolean setSecondary()
        {
            for (int i = 0; i <= 3; i++)
            {
                setSecondary(i);
            }
            return true;

        }

        private Boolean setSecondary(int i)
        {
            if (secondaryPropertyComboBoxes[i].SelectedItem != null && secondaryAmountComboBoxes[i].SelectedItem != null && 
                secondaryPropertyComboBoxes[i].SelectedIndex != 0 && secondaryAmountComboBoxes[i].SelectedIndex != 0)
            {
                RuneStat stat = new RuneStat();
                stat.setProperty(RuneStat.toProperty((string)secondaryPropertyComboBoxes[i].SelectedItem));
                stat.setAmount((int)secondaryAmountComboBoxes[i].SelectedItem);
                _rune.setSecondaryProperty(i, stat);
            }
            else
            {
                _rune.setSecondaryProperty(i, new RuneStat());
            }
            return true;
        }

        private Boolean displaySecondary()
        {
            List<RuneStat> stats = _rune.getCompiledSecondaryProperties();
            for (int i = 0; i < 4; i++)
            {
                if (i < stats.Count)
                {
                    RuneStat stat = stats[i];
                    secondaryAmountFinal[i].Text = stat.amount.ToString();
                    secondaryPropertyFinal[i].Text = stat.property.ToString();
                }
                else 
                {
                    secondaryAmountFinal[i].Text = "";
                    secondaryPropertyFinal[i].Text = "";
                }
            }
            return true;
        }

        private Boolean setUpgrades()
        {
            for (int i = 0; i <= 3; i++)
            {
                setUpgrade(i);
            }
            return true;
        }

        private Boolean setUpgrade(int i)
        {
            if (upgradePropertyComboBoxes[i].SelectedItem != null && upgradeAmountComboBoxes[i].SelectedItem != null && 
                upgradePropertyComboBoxes[i].SelectedIndex != 0 && upgradeAmountComboBoxes[i].SelectedIndex != 0)
            {
                RuneStat stat = new RuneStat();
                stat.setProperty(RuneStat.toProperty((string)upgradePropertyComboBoxes[i].SelectedItem));
                stat.setAmount((int)upgradeAmountComboBoxes[i].SelectedItem);
                _rune.setUpgradeProperty(i, stat);
            }
            else
            {
                _rune.setUpgradeProperty(i, new RuneStat());
                return false;
            }
            return true;
        }

        private void RuneCalculateButton_Click(object sender, RoutedEventArgs e)
        {
            calculate();
           
            if (DialogFinished != null)
                DialogFinished(this, new WindowEventArgs(_rune.ToString()));

            
        }



        //
        //
        // Combo box bindings
        //
        //

        private void bindOtherComboBoxes()
        {

            StarCombo.ItemsSource = StarList;
            StarCombo.SelectedIndex = 0;
            TypeCombo.ItemsSource = TypeList;
            TypeCombo.SelectedIndex = 0;
            LevelCombo.ItemsSource = LevelList;
            LevelCombo.SelectedIndex = 0;
        }

        private void bindMainPropertyList()
        {
            PrimaryPropertyCombo.ItemsSource = mainPropertyList;
            InnatePropertyCombo.ItemsSource = mainPropertyList;
            SecondaryPropertyCombo1.ItemsSource = mainPropertyList;
            SecondaryPropertyCombo2.ItemsSource = mainPropertyList;
            SecondaryPropertyCombo3.ItemsSource = mainPropertyList;
            SecondaryPropertyCombo4.ItemsSource = mainPropertyList;
        }

        private void resetMainPropertyCounts()
        {
            PrimaryPropertyCombo.SelectedIndex = 0;
            InnatePropertyCombo.SelectedIndex = 0;
            SecondaryPropertyCombo1.SelectedIndex = 0;
            SecondaryPropertyCombo2.SelectedIndex = 0;
            SecondaryPropertyCombo3.SelectedIndex = 0;
            SecondaryPropertyCombo4.SelectedIndex = 0;

            InnateAmountCombo.SelectedIndex = 0;
            SecondaryAmountCombo1.SelectedIndex = 0;
            SecondaryAmountCombo2.SelectedIndex = 0;
            SecondaryAmountCombo3.SelectedIndex = 0;
            SecondaryAmountCombo4.SelectedIndex = 0;
        }


        private void bindUpgradeAmount()
        {
            InnateAmountCombo.ItemsSource = UpgradeAmount;
            SecondaryAmountCombo1.ItemsSource = UpgradeAmount;
            SecondaryAmountCombo2.ItemsSource = UpgradeAmount;
            SecondaryAmountCombo3.ItemsSource = UpgradeAmount;
            SecondaryAmountCombo4.ItemsSource = UpgradeAmount;

            Upgrade3AmountCombo.ItemsSource = UpgradeAmount;
            Upgrade6AmountCombo.ItemsSource = UpgradeAmount;
            Upgrade9AmountCombo.ItemsSource = UpgradeAmount;
            Upgrade12AmountCombo.ItemsSource = UpgradeAmount;
        }


        private void bindUpgradePropertyList()
        {

            Upgrade3PropertyCombo.ItemsSource = upgradePropertyList;
            Upgrade6PropertyCombo.ItemsSource = upgradePropertyList;
            Upgrade9PropertyCombo.ItemsSource = upgradePropertyList;
            Upgrade12PropertyCombo.ItemsSource = upgradePropertyList;

        }


        private void resetUpgradePropertyCounts()
        {
            Upgrade3PropertyCombo.SelectedIndex = 0;
            Upgrade6PropertyCombo.SelectedIndex = 0;
            Upgrade9PropertyCombo.SelectedIndex = 0;
            Upgrade12PropertyCombo.SelectedIndex = 0;

            Upgrade3AmountCombo.SelectedIndex = 0;
            Upgrade6AmountCombo.SelectedIndex = 0;
            Upgrade9AmountCombo.SelectedIndex = 0;
            Upgrade12AmountCombo.SelectedIndex = 0;

        }




        //
        //
        // Combo box selections
        //
        //

        private void PrimaryPropertyCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PrimaryPropertyCombo.SelectedItem != null)
            {
                setPrimary();

                displayPrimary();
            }
        }

        private void SecondaryPropertyCombo1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SecondaryPropertyCombo1.SelectedItem != null)
            {
                int[] range = Rune.getSecondaryIncrementRange(RuneStat.parseProperty((string)SecondaryPropertyCombo1.SelectedItem), _rune.star);
                SecondaryAmountCombo1.ItemsSource = Enumerable.Range(range[0], range[1] - range[0] + 1);
                
            }
        }

        private void SecondaryPropertyCombo2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SecondaryPropertyCombo2.SelectedItem != null)
            {
                int[] range = Rune.getSecondaryIncrementRange(RuneStat.parseProperty((string)SecondaryPropertyCombo2.SelectedItem), _rune.star);
                SecondaryAmountCombo2.ItemsSource = Enumerable.Range(range[0], range[1] - range[0] + 1); 
                
            }
        }

        private void SecondaryPropertyCombo3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SecondaryPropertyCombo3.SelectedItem != null)
            {
                int[] range = Rune.getSecondaryIncrementRange(RuneStat.parseProperty((string)SecondaryPropertyCombo3.SelectedItem), _rune.star);
                SecondaryAmountCombo3.ItemsSource = Enumerable.Range(range[0], range[1] - range[0] + 1);
            }
        }

        private void SecondaryPropertyCombo4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SecondaryPropertyCombo4.SelectedItem != null)
            {
                int[] range = Rune.getSecondaryIncrementRange(RuneStat.parseProperty((string)SecondaryPropertyCombo4.SelectedItem), _rune.star);
                SecondaryAmountCombo4.ItemsSource = Enumerable.Range(range[0], range[1] - range[0] + 1);
            }
        }

        private void InnatePropertyCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InnatePropertyCombo.SelectedItem != null)
            {
                int[] range = Rune.getSecondaryIncrementRange(RuneStat.parseProperty((string)InnatePropertyCombo.SelectedItem), _rune.star);
                InnateAmountCombo.ItemsSource = Enumerable.Range(range[0], range[1] - range[0] + 1);
            }
        }



        //
        //
        // Misc methods
        //
        //

        public static String listToString(List<int> lst)
        {
            String result = "";
            foreach(int item in lst) {
                result += item.ToString();
                result += " ";
            }
            return result;
        }

        public static String listToString(List<string> lst)
        {
            String result = "";
            foreach (string item in lst)
            {
                result += item;
                result += " ";
            }
            return result;
        }

        private void StarCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StarCombo.SelectedItem != null)
            {
                _rune.setStar(Int32.Parse((string)StarCombo.SelectedItem));
            }
        }

        private void LevelCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LevelCombo.SelectedItem != null)
            {
                _rune.setLevel(Int32.Parse((string)LevelCombo.SelectedItem));
            }
        }

        private void TypeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TypeCombo.SelectedItem != null)
            {
                _rune.setType((Rune.Type)Enum.Parse(typeof(Rune.Type), (string)TypeCombo.SelectedItem, true));
            }
        }

        private void InnateAmountCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InnateAmountCombo.SelectedItem != null && InnatePropertyCombo.SelectedItem != null)
            {
                setInherent();
                calculate();
                displayInherent();
            }
        }

        private void SecondaryAmountCombo1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SecondaryPropertyCombo1.SelectedItem != null && SecondaryAmountCombo1.SelectedItem != null)
            {
                setSecondary(0);
                calculate();
                displaySecondary();
            }
        }

        private void SecondaryAmountCombo2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SecondaryPropertyCombo2.SelectedItem != null && SecondaryAmountCombo2.SelectedItem != null)
            {
                setSecondary(1);
                calculate();
                displaySecondary();
            }
        }

        private void SecondaryAmountCombo3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SecondaryPropertyCombo3.SelectedItem != null && SecondaryAmountCombo3.SelectedItem != null)
            {
                setSecondary(2);
                calculate();
                displaySecondary();
            }
        }

        private void SecondaryAmountCombo4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SecondaryPropertyCombo4.SelectedItem != null && SecondaryAmountCombo4.SelectedItem != null)
            {
                setSecondary(3);
                calculate();
                displaySecondary();
            }
        }

        public void unserialize(string info) 
        {

        }
    }
}