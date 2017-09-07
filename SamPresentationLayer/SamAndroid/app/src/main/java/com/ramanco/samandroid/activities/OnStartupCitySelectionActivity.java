package com.ramanco.samandroid.activities;

import android.content.Intent;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
import android.os.Bundle;
import android.text.TextUtils;

import com.ramanco.samandroid.R;
import com.ramanco.samandroid.utils.ExceptionManager;
import com.ramanco.samandroid.fragments.CitySelectionFragment;
import com.ramanco.samandroid.utils.PrefUtil;

public class OnStartupCitySelectionActivity extends BaseActivity {

    //region Ctors:
    public OnStartupCitySelectionActivity() {
    }
    //endregion

    //region Overrides:
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        try {

            super.onCreate(savedInstanceState);
            setContentView(R.layout.activity_city_selection);

            initActionBar(false, false, "");

            FragmentManager fragmentManager = getSupportFragmentManager();
            CitySelectionFragment fragmentCitySelection = new CitySelectionFragment();
            FragmentTransaction fragmentTransaction = fragmentManager.beginTransaction();
            fragmentTransaction.replace(R.id.fragment_placeholder, fragmentCitySelection);
            fragmentTransaction.commit();
            fragmentCitySelection.setOnCitySelectRunnable(new Runnable() {
                @Override
                public void run() {
                    try {
                        String customerInfo = PrefUtil.getCustomerInfo(OnStartupCitySelectionActivity.this);
                        if (TextUtils.isEmpty(customerInfo)) {
                            //region customer info step:
                            Intent intent = new Intent(OnStartupCitySelectionActivity.this, OnStartupCustomerInfoActivity.class);
                            OnStartupCitySelectionActivity.this.startActivity(intent);
                            finish();
                            //endregion
                        } else {
                            //region start main activity:
                            Intent intent = new Intent(OnStartupCitySelectionActivity.this, MainActivity.class);
                            startActivity(intent);
                            finish();
                            //endregion
                        }
                    } catch (Exception ex) {
                        ExceptionManager.handle(OnStartupCitySelectionActivity.this, ex);
                    }
                }
            });

        } catch (Exception ex) {
            ExceptionManager.handle(this, ex);
        }
    }
    //endregion

}
