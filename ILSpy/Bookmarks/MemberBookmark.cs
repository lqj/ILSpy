﻿// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using Mono.Cecil;

namespace ICSharpCode.ILSpy.Bookmarks
{
	/// <summary>
	/// Bookmark used to give additional operations for class members.
	/// Does not derive from SDBookmark because it is not stored in the central BookmarkManager,
	/// but only in the document's BookmarkManager.
	/// </summary>
	public class MemberBookmark : IBookmark
	{
		AstNode node;
		
		public AstNode Node {
			get {
				return node;
			}
		}
		
		public MemberBookmark(AstNode node)
		{
			this.node = node;
		}
		
		public virtual ImageSource Image {
			get {
				var attrNode = (AttributedNode)node;
				if (node is EnumMemberDeclaration)
					return GetMemberOverlayedImage(attrNode, MemberIcon.EnumValue);
				
				if (node is FieldDeclaration)
					return GetMemberOverlayedImage(attrNode, MemberIcon.Field);
				
				if (node is PropertyDeclaration)
					return GetMemberOverlayedImage(attrNode, MemberIcon.Property);
				
				if (node is EventDeclaration || node is CustomEventDeclaration)
					return GetMemberOverlayedImage(attrNode, MemberIcon.Event);
				
				if (node is IndexerDeclaration) 
					return GetMemberOverlayedImage(attrNode, MemberIcon.Indexer);
				
				if (node is OperatorDeclaration)
					return GetMemberOverlayedImage(attrNode, MemberIcon.Operator);
				
				if (node is ConstructorDeclaration || node is DestructorDeclaration)
					return GetMemberOverlayedImage(attrNode, MemberIcon.Constructor);
				
				return GetMemberOverlayedImage(attrNode, MemberIcon.Method);
			}
		}
		
		ImageSource GetMemberOverlayedImage(AttributedNode attrNode, MemberIcon icon)
		{
			switch (attrNode.Modifiers & Modifiers.VisibilityMask) {
				case Modifiers.Protected:
					return Images.GetIcon(icon, AccessOverlayIcon.Protected, (attrNode.Modifiers & Modifiers.Static) == Modifiers.Static);
				case Modifiers.Private:
					return Images.GetIcon(icon, AccessOverlayIcon.Private, (attrNode.Modifiers & Modifiers.Static) == Modifiers.Static);
				case Modifiers.Internal:
					return Images.GetIcon(icon, AccessOverlayIcon.Internal, (attrNode.Modifiers & Modifiers.Static) == Modifiers.Static);
			}
			
			return Images.GetIcon(icon, AccessOverlayIcon.Public, (attrNode.Modifiers & Modifiers.Static) == Modifiers.Static);
		}
		
		public int LineNumber {
			get {
				var t = node.Annotation<Tuple<int, int>>();
				if (t != null)
					return t.Item1;
				return 0;
			}
		}
		
		public virtual void MouseDown(MouseButtonEventArgs e)
		{
		}
		
		public virtual void MouseUp(MouseButtonEventArgs e)
		{
		}
		
		int IBookmark.ZOrder {
			get { return -10; }
		}
		
		bool IBookmark.CanDragDrop {
			get { return false; }
		}
		
		void IBookmark.Drop(int lineNumber)
		{
			throw new NotSupportedException();
		}
	}
	
	public class TypeBookmark : MemberBookmark
	{
		public TypeBookmark(AstNode node) : base (node)
		{
		}
		
		public override ImageSource Image {
			get {
				var attrNode = (AttributedNode)Node;
				
				if (Node is DelegateDeclaration)
					return GetTypeOverlayedImage(attrNode, TypeIcon.Delegate);
				
				if (Node is TypeDeclaration) {
					var n = Node as TypeDeclaration;
					switch (n.ClassType)
					{
						case ClassType.Delegate:
							return GetTypeOverlayedImage(attrNode, TypeIcon.Delegate);
						case ClassType.Enum:
							return GetTypeOverlayedImage(attrNode, TypeIcon.Enum);
						case ClassType.Struct:
							return GetTypeOverlayedImage(attrNode, TypeIcon.Struct);
						case ClassType.Interface:
							return GetTypeOverlayedImage(attrNode, TypeIcon.Interface);
					}
				}
				
				if ((attrNode.Modifiers & Modifiers.Static) == Modifiers.Static)
					return GetTypeOverlayedImage(attrNode, TypeIcon.StaticClass);
				
				return GetTypeOverlayedImage(attrNode, TypeIcon.Class);
			}
		}
		
		ImageSource GetTypeOverlayedImage(AttributedNode attrNode, TypeIcon icon)
		{
			switch (attrNode.Modifiers & Modifiers.VisibilityMask) {
				case Modifiers.Protected:
					return Images.GetIcon(icon, AccessOverlayIcon.Protected);
				case Modifiers.Private:
					return Images.GetIcon(icon, AccessOverlayIcon.Private);
				case Modifiers.Internal:
					return Images.GetIcon(icon, AccessOverlayIcon.Internal);
			}
			
			return Images.GetIcon(icon, AccessOverlayIcon.Public);
		}
	}
}
