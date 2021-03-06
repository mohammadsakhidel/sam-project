package com.ramanco.samandroid.activities;

import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
import android.text.TextUtils;

import com.ramanco.samandroid.R;
import com.ramanco.samandroid.fragments.CitySelectionFragment;
import com.ramanco.samandroid.utils.ExceptionManager;
import com.ramanco.samandroid.utils.PrefUtil;

public class CitySelectionActivity extends BaseActivity {

    //region Overrides:
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        try {

            super.onCreate(savedInstanceState);
            setContentView(R.layout.activity_city_selection);

            initActionBar(false, true, "");

            FragmentManager fragmentManager = getSupportFragmentManager();
            CitySelectionFragment fragmentCitySelection = new CitySelectionFragment();
            FragmentTransaction fragmentTransaction = fragmentManager.beginTransaction();
            fragmentTransaction.replace(R.id.fragment_placeholder, fragmentCitySelection);
            fragmentTransaction.commit();
            fragmentCitySelection.setOnCitySelectRunnable(new Runnable() {
                @Override
                public void run() {
                    try {
                        finish();
                    } catch (Exception ex) {
                        ExceptionManager.handle(CitySelectionActivity.this, ex);
                    }
                }
            });

        } catch (Exception ex) {
            ExceptionManager.handle(this, ex);
        }
    }
    //endregion

}
