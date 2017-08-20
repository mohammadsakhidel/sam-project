package com.ramanco.samandroid.code.base;

import android.app.Application;

import com.ramanco.samandroid.code.utils.ExceptionManager;
import com.ramanco.samandroid.code.utils.FontUtility;

public class AppContext extends Application {

    @Override
    public void onCreate() {
        super.onCreate();

        try {

            FontUtility.overrideFont(getApplicationContext(), "DEFAULT", "Samim.ttf");
            FontUtility.overrideFont(getApplicationContext(), "SERIF", "Shabnam.ttf");

        } catch (Exception e) {
            ExceptionManager.Handle(this, e);
        }

    }
}
