package com.ramanco.samandroid.activities;

import android.support.v4.widget.DrawerLayout;
import android.support.v7.app.ActionBarDrawerToggle;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.support.v7.widget.Toolbar;
import android.widget.TextView;

import com.ramanco.samandroid.R;
import com.ramanco.samandroid.code.utils.ExceptionManager;
import com.ramanco.samandroid.code.utils.FontUtility;

public class MainActivity extends BaseActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        try {
            super.onCreate(savedInstanceState);
            setContentView(R.layout.activity_main);

            //region ACTIONBAR:
            super.initActionBar(false, true, "");
            Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
            //endregion

            //region DRAWER:
            DrawerLayout drawer = (DrawerLayout) findViewById(R.id.root);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, 0, 0);
            drawer.addDrawerListener(toggle);
            toggle.syncState();
            //endregion

        } catch (Exception ex) {
            ExceptionManager.Handle(this, ex);
        }
    }

}
