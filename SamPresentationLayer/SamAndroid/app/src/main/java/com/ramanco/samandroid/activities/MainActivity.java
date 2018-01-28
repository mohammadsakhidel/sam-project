package com.ramanco.samandroid.activities;

import android.app.Notification;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.media.RingtoneManager;
import android.net.Uri;
import android.os.Handler;
import android.support.annotation.NonNull;
import android.support.design.widget.BottomNavigationView;
import android.support.design.widget.NavigationView;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
import android.support.v4.view.GravityCompat;
import android.support.v4.widget.DrawerLayout;
import android.support.v7.app.ActionBarDrawerToggle;
import android.os.Bundle;
import android.support.v4.app.NotificationCompat;
import android.support.v7.widget.Toolbar;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.view.WindowManager;
import android.widget.TextView;
import android.widget.Toast;

import com.google.gson.Gson;
import com.ramanco.samandroid.BuildConfig;
import com.ramanco.samandroid.R;
import com.ramanco.samandroid.api.dtos.AppVersionDto;
import com.ramanco.samandroid.api.dtos.CustomerDto;
import com.ramanco.samandroid.api.endpoints.SyncApiEndpoint;
import com.ramanco.samandroid.objects.KeyValuePair;
import com.ramanco.samandroid.utils.ApiUtil;
import com.ramanco.samandroid.utils.CityUtil;
import com.ramanco.samandroid.utils.ExceptionManager;
import com.ramanco.samandroid.fragments.HistoryFragment;
import com.ramanco.samandroid.fragments.SendConsolationFragment;
import com.ramanco.samandroid.fragments.TrackFragment;
import com.ramanco.samandroid.utils.PrefUtil;

import java.io.IOException;

import retrofit2.Response;

public class MainActivity extends BaseActivity {

    //region Fields:
    boolean backPressedOnce = false;
    //endregion

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
            //region check for updates:
            new Thread(new Runnable() {
                @Override
                public void run() {
                    try {
                        checkForUpdates();
                    } catch (Exception ex) {
                    }
                }
            }).start();
            //endregion
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

    @Override
    public void onBackPressed() {

        DrawerLayout drawer = (DrawerLayout) findViewById(R.id.root);
        FragmentManager fragmentManager = getSupportFragmentManager();
        int stackFragmentsCount = fragmentManager.getBackStackEntryCount();

        if (stackFragmentsCount > 0) {
            fragmentManager.popBackStack();
        } else if (drawer.isDrawerOpen(GravityCompat.START)) {
            drawer.closeDrawer(GravityCompat.START);
        } else {
            //region exit with wait:
            if (backPressedOnce) {
                super.onBackPressed();
                return;
            }

            backPressedOnce = true;
            Toast.makeText(this, getResources().getString(R.string.back_again_to_exit), Toast.LENGTH_SHORT).show();

            new Handler().postDelayed(new Runnable() {

                @Override
                public void run() {
                    backPressedOnce = false;
                }
            }, 2000);
            //endregion
        }
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

    private void checkForUpdates() throws IOException, PackageManager.NameNotFoundException {
        SyncApiEndpoint endpoint = ApiUtil.createEndpoint(SyncApiEndpoint.class);
        Response<AppVersionDto> response = endpoint.appVersion().execute();
        if (response.isSuccessful()) {
            final AppVersionDto dto = response.body();
            int appVersionCode = BuildConfig.VERSION_CODE;
            if (dto.getAppVersionCode() > appVersionCode) {
                MainActivity.this.runOnUiThread(new Runnable() {
                    @Override
                    public void run() {
                        try {
                            showUpdateNotification(dto);
                        } catch (Exception ex) {
                            ExceptionManager.handle(MainActivity.this, ex);
                        }
                    }
                });
            }
        }
    }

    public void showUpdateNotification(AppVersionDto dto) {

        Uri soundUri = RingtoneManager.getDefaultUri(RingtoneManager.TYPE_NOTIFICATION);
        Intent intent = new Intent(Intent.ACTION_VIEW);
        intent.setData(Uri.parse(dto.getNewVersionUrl()));
        PendingIntent pIntent = PendingIntent.getActivity(MainActivity.this, 0, intent, 0);

        Notification n  = new Notification.Builder(this)
                .setContentTitle(getResources().getString(R.string.msg_update_not_title))
                .setContentText(getResources().getString(R.string.msg_update_not_content))
                .setSmallIcon(R.drawable.ic_notification)
                .setSound(soundUri)
                .setContentIntent(pIntent)
                .setAutoCancel(true).build();

        NotificationManager notificationManager = (NotificationManager)
                getSystemService(NOTIFICATION_SERVICE);
        notificationManager.notify(0, n);

    }
    //endregion

}