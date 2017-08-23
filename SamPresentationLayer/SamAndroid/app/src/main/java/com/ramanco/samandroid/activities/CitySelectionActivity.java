package com.ramanco.samandroid.activities;

import android.content.Intent;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;

import com.ramanco.samandroid.R;
import com.ramanco.samandroid.utils.ExceptionManager;
import com.ramanco.samandroid.fragments.CitySelectionFragment;

public class CitySelectionActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        try {

            super.onCreate(savedInstanceState);
            setContentView(R.layout.activity_city_selection);

            FragmentManager fragmentManager = getSupportFragmentManager();
            CitySelectionFragment fragmentCitySelection = new CitySelectionFragment();
            FragmentTransaction fragmentTransaction = fragmentManager.beginTransaction();
            fragmentTransaction.replace(R.id.root, fragmentCitySelection);
            fragmentTransaction.commit();
            fragmentCitySelection.setOnCitySelectRunnable(new Runnable() {
                @Override
                public void run() {
                    try {
                        //region start main activity:
                        Intent intent = new Intent(CitySelectionActivity.this, MainActivity.class);
                        startActivity(intent);
                        finish();
                        //endregion
                    } catch (Exception ex) {
                        ExceptionManager.Handle(CitySelectionActivity.this, ex);
                    }
                }
            });

        } catch (Exception ex) {
            ExceptionManager.Handle(this, ex);
        }
    }
}
