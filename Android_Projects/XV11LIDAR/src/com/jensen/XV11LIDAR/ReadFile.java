package com.jensen.XV11LIDAR;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileInputStream;
import java.io.FilePermission;
import java.io.FileReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.ArrayList;

import android.os.Environment;
import android.util.Log;

//*********************************************
// ****File Reading Class******
public class ReadFile {
	ArrayList<String> storeValues = new ArrayList<String>();

	private static boolean D = false;     //used for deciding if to print debug messages
	private String fileName = "";
	private boolean isOpen = false;
	private File rootDir;
	private FileInputStream in;
	private InputStreamReader isr;
	private BufferedReader br;
	String tag = "";
	String dataTemp = "";
	// constructor
	public ReadFile( String name) {
		this.fileName = name;
		
	}

	public void openFile() throws IOException{

		//To get a source file on SDcard you must have it pushed there by file explorer through eclipse window->showView->other
		//also must have permissions in the manifest file
		
		try {

			rootDir = Environment.getExternalStorageDirectory();
			Log.e(tag, "root dir = " + rootDir + "/" + fileName);

			isOpen = true;

			in = new FileInputStream(rootDir + "/" + fileName);
			
			isr = new InputStreamReader(in);
			br = new BufferedReader(isr);
			
			Log.e(tag, "**OPENED PARSE FILE**");
			
			this.isOpen();
			

		} catch (IOException e) {
			e.printStackTrace();
			
		}
		
	} 
	
	
	public boolean isOpen() {
		if(D)
			Log.e(tag, "Is file Open? -->" + isOpen);
		
		return isOpen;
	}

	// file writer
	public void parseFile() throws IOException {

		

	}

	public int readValue() {
		int temp = 0;
		
		try {
			temp = br.read();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
		return temp;
	}
	
	public char [] readBuff() {
		char [] buff = new char [20];
		
		try {
			br.read(buff,0,14);
			
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
		
		return buff;
	}

	public void closeFile() throws IOException {
		isOpen = false;
		br.close();

		Log.e(tag, "File State:" + isOpen);
		Log.e(tag, "Closed File");
	}

} // end WriteFile class