package com.ramanco.samandroid.activities;

import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;

import com.ramanco.samandroid.R;
import com.ramanco.samandroid.code.utils.ExceptionManager;

public class LauncherActivity extends BaseActivity {

    //region Fields:
    private static final int DELAY_MILLS = 4000;
    //endregion

    //region Overrides:
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        try {

            super.onCreate(savedInstanceState);
            setContentView(R.layout.activity_launcher);

            //region delay and start main activity:
            new Thread(new Runnable() {
                @Override
                public void run() {
                    try {
                        Thread.sleep(DELAY_MILLS);
                        LauncherActivity.this.runOnUiThread(new Runnable() {
                            @Override
                            public void run() {
                                try {
                                    Intent intent = new Intent(LauncherActivity.this, MainActivity.class);
                                    LauncherActivity.this.startActivity(intent);
                                    finish();
                                } catch (Exception ex) {
                                    ExceptionManager.Handle(LauncherActivity.this, ex);
                                }
                            }
                        });
                    } catch (Exception ex) {
                        ExceptionManager.Handle(LauncherActivity.this, ex);
                    }
                }
            }).start();
            //endregion

        } catch (Exception ex) {
            ExceptionManager.Handle(this, ex);
        }
    }
    //endregion

}
