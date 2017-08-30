package com.ramanco.samandroid.activities;

import android.support.annotation.NonNull;
import android.support.design.widget.BottomNavigationView;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
import android.support.v4.widget.DrawerLayout;
import android.support.v7.app.ActionBarDrawerToggle;
import android.os.Bundle;
import android.support.v7.widget.Toolbar;
import android.view.Menu;
import android.view.MenuItem;

import com.ramanco.samandroid.R;
import com.ramanco.samandroid.utils.ExceptionManager;
import com.ramanco.samandroid.fragments.HistoryFragment;
import com.ramanco.samandroid.fragments.SendConsolationFragment;
import com.ramanco.samandroid.fragments.SettingsFragment;

public class MainActivity extends BaseActivity {

    //region Overrides:
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

            //region Bottom NavigationView:
            //region navigation selected item changed:
            BottomNavigationView navigation = (BottomNavigationView) findViewById(R.id.navigation);
            navigation.setOnNavigationItemSelectedListener(new BottomNavigationView.OnNavigationItemSelectedListener() {
                @Override
                public boolean onNavigationItemSelected(@NonNull MenuItem item) {
                    try {
                        //region choose fragment:
                        Fragment fragment = null;
                        int selectedItemId = item.getItemId();
                        switch (selectedItemId) {
                            case R.id.mi_send_consolation:
                                fragment = new SendConsolationFragment();
                                break;
                            case R.id.mi_history:
                                fragment = new HistoryFragment();
                                break;
                            case R.id.mi_settings:
                                fragment = new SettingsFragment();
                                break;
                        }
                        //endregion
                        //region show fragment:
                        if (fragment != null)
                            replaceFragment(fragment);
                        //endregion
                        return true;
                    } catch (Exception ex) {
                        ExceptionManager.handle(MainActivity.this, ex);
                        return false;
                    }
                }
            });
            //endregion
            //region set default navigation item:
            int defaultFragmentIndex = 1;
            Fragment defaultFragment = new HistoryFragment();
            setDefaultFragment(defaultFragmentIndex, defaultFragment);
            //endregion
            //endregion

        } catch (Exception ex) {
            ExceptionManager.handle(this, ex);
        }
    }
    //endregion

    //region Methods:
    private void replaceFragment(Fragment fragment) {
        FragmentManager fragmentManager = getSupportFragmentManager();
        FragmentTransaction fragmentTransaction = fragmentManager.beginTransaction();
        fragmentTransaction.replace(R.id.ly_main_content, fragment);
        fragmentTransaction.commit();
    }

    private void setDefaultFragment(int defaultFragmentIndex, Fragment defaultFragment) {
        BottomNavigationView navigation = (BottomNavigationView) findViewById(R.id.navigation);
        Menu navigationMenu = navigation.getMenu();
        MenuItem navigationMenuItem = navigationMenu.getItem(defaultFragmentIndex);
        navigationMenuItem.setChecked(true);
        replaceFragment(defaultFragment);
    }

    //endregion

}