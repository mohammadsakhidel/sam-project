package com.ramanco.samandroid.fragments;


import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Toast;

import com.ramanco.samandroid.R;
import com.ramanco.samandroid.code.utils.ExceptionManager;

public class SettingsFragment extends Fragment {


    public SettingsFragment() {
        // Required empty public constructor
    }


    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {

        View fragmentView = inflater.inflate(R.layout.fragment_settings, container, false);

        try {

        } catch (Exception ex) {
            ExceptionManager.Handle(getActivity(), ex);
        }

        return fragmentView;
    }

}
