package com.ramanco.samandroid.activities;

import android.content.Intent;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
import android.os.Bundle;

import com.google.gson.Gson;
import com.ramanco.samandroid.R;
import com.ramanco.samandroid.api.dtos.CustomerDto;
import com.ramanco.samandroid.fragments.CustomerSpecsFragment;
import com.ramanco.samandroid.utils.ExceptionManager;
import com.ramanco.samandroid.utils.PrefUtil;

public class OnStartupCustomerInfoActivity extends BaseActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        try {

            super.onCreate(savedInstanceState);
            setContentView(R.layout.activity_customer_info);

            initActionBar(false, false, "");

            //region load first step fragment:
            final CustomerSpecsFragment fragment = new CustomerSpecsFragment();
            fragment.setOnOkAction(new Runnable() {
                @Override
                public void run() {
                    try {
                        //region save customer info:
                        CustomerDto customer = fragment.getCustomer();
                        String json = new Gson().toJson(customer);
                        PrefUtil.setCustomerInfo(OnStartupCustomerInfoActivity.this, json);
                        //endregion
                        //region show main activity:
                        Intent intent = new Intent(OnStartupCustomerInfoActivity.this, MainActivity.class);
                        OnStartupCustomerInfoActivity.this.startActivity(intent);
                        OnStartupCustomerInfoActivity.this.finish();
                        //endregion
                    } catch (Exception ex) {
                        ExceptionManager.handle(OnStartupCustomerInfoActivity.this, ex);
                    }
                }
            });
            FragmentManager fm = getSupportFragmentManager();
            FragmentTransaction transaction = fm.beginTransaction();
            transaction.replace(R.id.fragment_placeholder, fragment);
            transaction.commit();
            //endregion

        } catch (Exception ex) {
            ExceptionManager.handle(this, ex);
        }
    }

}
