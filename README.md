# CSharp-BinaryTree
Author: WildstarCodeworks

Created: 11/15/2021

Last Modified: 11/17/2021


Created as both a self-educational project and a structure I needed, this binary tree
is designed to organize your data in a the expected way, but also carries along a payload of other
information that the user determines on initialization. In this way a binary tree can also return
indices, strings, custom objects, or whatever the author decides.

The code uses the Last-In-First-Out rule for deleting entries from a payload and keeps track of
its size and unique nodes when a users adds or removes a node. It is not recommended that users
use or modify the code to be First-In-First-Out as the runtime is abyssmal.

The code alsoa allows the user to decide how they would like nodes with 2 children to be replaced.
The two rules it offers are:

Predecessor: It will replace the deleted node with the largest node found by searching its left child.

Successor: It will replace the deleted node with the smallest node found by searching its right child.
This can be switched at any time with "SwitchModes()" and is, by default, "predecessor".
 
The code also keeps track of the parent of the node allowing users the option to trace up with ease.
 
I hope you enjoy this work. Thanks for reading! Code on.
