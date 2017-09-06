package com.ramanco.samandroid.fragments;


import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import com.ramanco.samandroid.R;
import com.ramanco.samandroid.utils.ExceptionManager;

public class TrackFragment extends Fragment {


    public TrackFragment() {
        // Required empty public constructor
    }


    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {

        View fragmentView = inflater.inflate(R.layout.fragment_track, container, false);

        try {

        } catch (Exception ex) {
            ExceptionManager.handle(getActivity(), ex);
        }

        return fragmentView;
    }

}
