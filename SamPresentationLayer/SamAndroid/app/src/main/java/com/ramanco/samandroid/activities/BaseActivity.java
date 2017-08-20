package com.ramanco.samandroid.activities;

import android.support.v7.app.ActionBar;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.text.TextUtils;

import com.ramanco.samandroid.R;

public class BaseActivity extends AppCompatActivity {

    //region Methods:
    public void initActionBar(boolean showAppTitle, boolean showBackButton, String title) {

        setSupportActionBar((Toolbar) findViewById(R.id.toolbar));

        ActionBar actionbar = getSupportActionBar();
        if (actionbar != null) {
            actionbar.setDisplayShowTitleEnabled(showAppTitle);
            actionbar.setDisplayShowHomeEnabled(showBackButton);
            actionbar.setDisplayHomeAsUpEnabled(showBackButton);
            if (!TextUtils.isEmpty(title))
                actionbar.setTitle(title);
        }

    }
    //endregion

}
