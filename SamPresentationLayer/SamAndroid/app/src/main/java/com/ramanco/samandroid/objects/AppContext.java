package com.ramanco.samandroid.objects;

import android.app.Application;

import com.ramanco.samandroid.utils.ExceptionManager;
import com.ramanco.samandroid.utils.FontUtility;

public class AppContext extends Application {

    @Override
    public void onCreate() {
        super.onCreate();

        try {

            FontUtility.overrideFont(getApplicationContext(), "DEFAULT", "Samim.ttf");
            FontUtility.overrideFont(getApplicationContext(), "SERIF", "Samim.ttf");

        } catch (Exception e) {
            ExceptionManager.handle(this, e);
        }

    }
}
