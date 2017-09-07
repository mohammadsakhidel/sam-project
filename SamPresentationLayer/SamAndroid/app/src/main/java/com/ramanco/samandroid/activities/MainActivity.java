package com.ramanco.samandroid.activities;

import android.content.Intent;
import android.support.annotation.NonNull;
import android.support.design.widget.BottomNavigationView;
import android.support.design.widget.NavigationView;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
import android.support.v4.widget.DrawerLayout;
import android.support.v7.app.ActionBarDrawerToggle;
import android.os.Bundle;
import android.support.v7.widget.Toolbar;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import com.google.gson.Gson;
import com.ramanco.samandroid.R;
import com.ramanco.samandroid.api.dtos.CustomerDto;
import com.ramanco.samandroid.objects.KeyValuePair;
import com.ramanco.samandroid.utils.CityUtil;
import com.ramanco.samandroid.utils.ExceptionManager;
import com.ramanco.samandroid.fragments.HistoryFragment;
import com.ramanco.samandroid.fragments.SendConsolationFragment;
import com.ramanco.samandroid.fragments.TrackFragment;
import com.ramanco.samandroid.utils.PrefUtil;

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
                                fragment = new TrackFragment();
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

            Menu navMenu = navigation.getMenu();
            MenuItem navMenuItem = navMenu.getItem(1);
            navMenuItem.setChecked(true);
            //endregion

        } catch (Exception ex) {
            ExceptionManager.handle(this, ex);
        }
    }

    @Override
    protected void onStart() {
        try {
            //region set user name:
            NavigationView navigationView = (NavigationView) findViewById(R.id.drawer_nav_view);
            View headerView = navigationView.getHeaderView(0);
            if (headerView != null) {
                String customerJson = PrefUtil.getCustomerInfo(this);
                CustomerDto customer = new Gson().fromJson(customerJson, CustomerDto.class);
                TextView tvUserName = (TextView) headerView.findViewById(R.id.tv_user_fullname);
                tvUserName.setText(String.format("%s: %s",
                        this.getResources().getString(R.string.user),
                        customer.getFullName()));
                tvUserName.setOnClickListener(new View.OnClickListener() {
                    @Override
                    public void onClick(View v) {
                        try {
                            Intent intent = new Intent(MainActivity.this, CustomerInfoActivity.class);
                            MainActivity.this.startActivity(intent);
                            DrawerLayout drawer = (DrawerLayout) findViewById(R.id.root);
                            drawer.closeDrawers();
                        } catch (Exception ex) {
                            ExceptionManager.handle(MainActivity.this, ex);
                        }
                    }
                });
            }
            //endregion
            //region set city menu item:
            int cityId = PrefUtil.getCityID(this);
            KeyValuePair selectedCity = CityUtil.find(this, cityId);
            if (selectedCity != null) {
                Menu navMenu = navigationView.getMenu();
                MenuItem navMenuItem = navMenu.findItem(R.id.mi_city);
                navMenuItem.setTitle(String.format("%s: %s",
                        getResources().getString(R.string.city),
                        selectedCity.getValue()));

                navMenuItem.setOnMenuItemClickListener(new MenuItem.OnMenuItemClickListener() {
                    @Override
                    public boolean onMenuItemClick(MenuItem item) {
                        try {

                            Intent intent = new Intent(MainActivity.this, CitySelectionActivity.class);
                            MainActivity.this.startActivity(intent);
                            DrawerLayout drawer = (DrawerLayout) findViewById(R.id.root);
                            drawer.closeDrawers();

                            return true;
                        } catch (Exception ex) {
                            ExceptionManager.handle(MainActivity.this, ex);
                            return false;
                        }
                    }
                });
            }
            //endregion
        } catch (Exception ex) {
            ExceptionManager.handle(this, ex);
        }

        super.onStart();
    }
    //endregion

    //region Methods:
    public void replaceFragment(Fragment fragment) {
        ViewGroup lyMainContent = (ViewGroup) findViewById(R.id.ly_main_content);
        lyMainContent.removeAllViews();

        FragmentManager fragmentManager = getSupportFragmentManager();
        FragmentTransaction fragmentTransaction = fragmentManager.beginTransaction();
        fragmentTransaction.replace(R.id.ly_main_content, fragment);
        fragmentTransaction.commit();
    }

    public void setBottomNavigationIndex(int index) {
        BottomNavigationView navigation = (BottomNavigationView) findViewById(R.id.navigation);
        Menu navigationMenu = navigation.getMenu();
        MenuItem navigationMenuItem = navigationMenu.getItem(index);
        navigationMenuItem.setChecked(true);
    }
    //endregion

}