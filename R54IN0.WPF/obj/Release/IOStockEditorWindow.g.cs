﻿#pragma checksum "..\..\IOStockEditorWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "6FC81A64D308DFA91BCE3F7B05A0B3E9"
//------------------------------------------------------------------------------
// <auto-generated>
//     이 코드는 도구를 사용하여 생성되었습니다.
//     런타임 버전:4.0.30319.42000
//
//     파일 내용을 변경하면 잘못된 동작이 발생할 수 있으며, 코드를 다시 생성하면
//     이러한 변경 내용이 손실됩니다.
// </auto-generated>
//------------------------------------------------------------------------------

using MahApps.Metro.Controls;
using R54IN0;
using R54IN0.WPF;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace R54IN0.WPF {
    
    
    /// <summary>
    /// IOStockEditorWindow
    /// </summary>
    public partial class IOStockEditorWindow : MahApps.Metro.Controls.MetroWindow, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\IOStockEditorWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal R54IN0.WPF.IOStockEditorWindow @this;
        
        #line default
        #line hidden
        
        
        #line 97 "..\..\IOStockEditorWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TreeViewEx FinderTreeView;
        
        #line default
        #line hidden
        
        
        #line 304 "..\..\IOStockEditorWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ItemAddButton;
        
        #line default
        #line hidden
        
        
        #line 305 "..\..\IOStockEditorWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ItemRemoveButton;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/R54IN0.WPF;component/iostockeditorwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\IOStockEditorWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.@this = ((R54IN0.WPF.IOStockEditorWindow)(target));
            return;
            case 2:
            this.FinderTreeView = ((System.Windows.Controls.TreeViewEx)(target));
            return;
            case 3:
            this.ItemAddButton = ((System.Windows.Controls.Button)(target));
            
            #line 304 "..\..\IOStockEditorWindow.xaml"
            this.ItemAddButton.Click += new System.Windows.RoutedEventHandler(this.ItemAddButton_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.ItemRemoveButton = ((System.Windows.Controls.Button)(target));
            
            #line 305 "..\..\IOStockEditorWindow.xaml"
            this.ItemRemoveButton.Click += new System.Windows.RoutedEventHandler(this.ItemRemoveButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

