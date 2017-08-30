package com.ramanco.samandroid.fragments;


import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import com.ramanco.samandroid.R;

public class PreviewStepFragment extends Fragment {

    //region Fields:
    SendConsolationFragment parentView;
    //endregion

    //region Ctors:
    public PreviewStepFragment() {
    }
    //endregion

    //region Overrides:
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        return inflater.inflate(R.layout.fragment_preview_step, container, false);
    }
    //endregion

    //region Getters & Setters:

    public SendConsolationFragment getParentView() {
        return parentView;
    }

    public void setParentView(SendConsolationFragment parentView) {
        this.parentView = parentView;
    }

    //endregion

}
