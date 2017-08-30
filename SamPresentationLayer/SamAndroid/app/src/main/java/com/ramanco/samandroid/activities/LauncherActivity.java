package com.ramanco.samandroid.activities;

import android.content.Intent;
import android.os.Bundle;
import com.ramanco.samandroid.R;
import com.ramanco.samandroid.utils.ExceptionManager;
import com.ramanco.samandroid.utils.PrefUtil;

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
                                    int cityId = PrefUtil.getCityID(LauncherActivity.this);
                                    if (cityId > 0) {
                                        //region start main activity:
                                        Intent intent = new Intent(LauncherActivity.this, MainActivity.class);
                                        LauncherActivity.this.startActivity(intent);
                                        finish();
                                        //endregion
                                    } else {
                                        //region start city selection activity:
                                        Intent intent = new Intent(LauncherActivity.this, CitySelectionActivity.class);
                                        LauncherActivity.this.startActivity(intent);
                                        finish();
                                        //endregion
                                    }
                                } catch (Exception ex) {
                                    ExceptionManager.handle(LauncherActivity.this, ex);
                                }
                            }
                        });
                    } catch (Exception ex) {
                        ExceptionManager.handle(LauncherActivity.this, ex);
                    }
                }
            }).start();
            //endregion

        } catch (Exception ex) {
            ExceptionManager.handle(this, ex);
        }
    }
    //endregion

}