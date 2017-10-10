package com.ramanco.samandroid.fragments;


import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageButton;

import com.ramanco.samandroid.R;
import com.ramanco.samandroid.activities.OnStartupCitySelectionActivity;
import com.ramanco.samandroid.api.dtos.MosqueDto;
import com.ramanco.samandroid.api.dtos.ObitDto;
import com.ramanco.samandroid.api.dtos.TemplateDto;
import com.ramanco.samandroid.utils.ExceptionManager;
import com.ramanco.samandroid.utils.PrefUtil;

public class SendConsolationFragment extends Fragment {

    //region Ctors:
    public SendConsolationFragment() {
    }
    //endregion

    //region Fields:
    private MosqueDto selectedMosque;
    private ObitDto selectedObit;
    private TemplateDto selectedTemplate;
    private String templateInfo;
    private int createdConsolationId;
    private String createdConsolationTN;
    private boolean nextVisible = false;
    private boolean prevVisible = false;
    private Runnable onNextClickListener;
    private Runnable onPreviousClickListener;
    private boolean loadFromPreview = false;
    //endregion

    //region Overrides:
    @Override
    public void onStart() {
        try {
            super.onStart();

            //region check city id:
            int cityId = PrefUtil.getCityID(getActivity());
            if (cityId <= 0) {
                Intent intent = new Intent(getActivity(), OnStartupCitySelectionActivity.class);
                startActivity(intent);
            }
            //endregion

        } catch (Exception ex) {
            ExceptionManager.handle(getActivity(), ex);
        }
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        View fragmentView = inflater.inflate(R.layout.fragment_send_consolation, container, false);

        try {

            updateNavButtonsStat();
            if (!loadFromPreview)
                showMosqueSelectionStep();
            else
                showPreviewStep();

            //region next prev on click:
            ImageButton btnNext = (ImageButton) fragmentView.findViewById(R.id.btn_next);
            ImageButton btnPrev = (ImageButton) fragmentView.findViewById(R.id.btn_prev);
            btnNext.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    try {
                        if (getOnNextClickListener() != null)
                            onNextClickListener.run();
                    } catch (Exception ex) {
                        ExceptionManager.handle(getActivity(), ex);
                    }
                }
            });
            btnPrev.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    try {
                        if (getOnPreviousClickListener() != null)
                            onPreviousClickListener.run();
                    } catch (Exception ex) {
                        ExceptionManager.handle(getActivity(), ex);
                    }
                }
            });
            //endregion

        } catch (Exception ex) {
            ExceptionManager.handle(getActivity(), ex);
        }

        return fragmentView;
    }
    //endregion

    //region Methods:
    private void updateNavButtonsStat() {
        View v = getView();
        if (v != null) {
            ImageButton btnNext = (ImageButton) v.findViewById(R.id.btn_next);
            ImageButton btnPrev = (ImageButton) v.findViewById(R.id.btn_prev);

            btnNext.setVisibility((isNextVisible() ? View.VISIBLE : View.INVISIBLE));
            btnPrev.setVisibility((isPrevVisible() ? View.VISIBLE : View.INVISIBLE));
        }
    }

    public void showMosqueSelectionStep() {
        MosqueSelectionFragment fragment = new MosqueSelectionFragment();
        fragment.setParentView(this);

        FragmentManager fragmentManager = getActivity().getSupportFragmentManager();
        FragmentTransaction transaction = fragmentManager.beginTransaction();
        transaction.replace(R.id.fragment_placeholder, fragment);
        transaction.commit();
    }

    public void showObitSelectionStep() {
        ObitSelectionFragment fragment = new ObitSelectionFragment();
        fragment.setParentView(this);

        FragmentManager fragmentManager = getActivity().getSupportFragmentManager();
        FragmentTransaction transaction = fragmentManager.beginTransaction();
        transaction.replace(R.id.fragment_placeholder, fragment);
        transaction.commit();
    }

    public void showTemplateSelectionStep() {
        TemplateSelectionFragment fragment = new TemplateSelectionFragment();
        fragment.setParentView(this);

        FragmentManager fragmentManager = getActivity().getSupportFragmentManager();
        FragmentTransaction transaction = fragmentManager.beginTransaction();
        transaction.replace(R.id.fragment_placeholder, fragment);
        transaction.commit();
    }

    public void showTemplateFieldsStep() {
        TemplateFieldsFragment fragment = new TemplateFieldsFragment();
        fragment.setParentView(this);

        FragmentManager fragmentManager = getActivity().getSupportFragmentManager();
        FragmentTransaction transaction = fragmentManager.beginTransaction();
        transaction.replace(R.id.fragment_placeholder, fragment);
        transaction.commit();
    }

    public void showPreviewStep() {
        PreviewStepFragment fragment = new PreviewStepFragment();
        fragment.setParentView(this);

        FragmentManager fragmentManager = getActivity().getSupportFragmentManager();
        FragmentTransaction transaction = fragmentManager.beginTransaction();
        transaction.replace(R.id.fragment_placeholder, fragment);
        transaction.commit();
    }
    //endregion

    //region Getters & Setters:
    public MosqueDto getSelectedMosque() {
        return selectedMosque;
    }

    public void setSelectedMosque(MosqueDto selectedMosque) {
        this.selectedMosque = selectedMosque;
    }

    public ObitDto getSelectedObit() {
        return selectedObit;
    }

    public void setSelectedObit(ObitDto selectedObit) {
        this.selectedObit = selectedObit;
    }

    public boolean isNextVisible() {
        return nextVisible;
    }

    public void setNextVisible(boolean nextVisible) {
        this.nextVisible = nextVisible;
        updateNavButtonsStat();
    }

    public boolean isPrevVisible() {
        return prevVisible;
    }

    public void setPrevVisible(boolean prevVisible) {
        this.prevVisible = prevVisible;
        updateNavButtonsStat();
    }

    public Runnable getOnNextClickListener() {
        return onNextClickListener;
    }

    public void setOnNextClickListener(Runnable onNextClickListener) {
        this.onNextClickListener = onNextClickListener;
    }

    public Runnable getOnPreviousClickListener() {
        return onPreviousClickListener;
    }

    public void setOnPreviousClickListener(Runnable onPreviousClickListener) {
        this.onPreviousClickListener = onPreviousClickListener;
    }

    public TemplateDto getSelectedTemplate() {
        return selectedTemplate;
    }

    public void setSelectedTemplate(TemplateDto selectedTemplate) {
        this.selectedTemplate = selectedTemplate;
    }

    public String getTemplateInfo() {
        return templateInfo;
    }

    public void setTemplateInfo(String templateInfo) {
        this.templateInfo = templateInfo;
    }

    public int getCreatedConsolationId() {
        return createdConsolationId;
    }

    public void setCreatedConsolationId(int createdConsolationId) {
        this.createdConsolationId = createdConsolationId;
    }

    public boolean isLoadFromPreview() {
        return loadFromPreview;
    }

    public void setLoadFromPreview(boolean loadFromPreview) {
        this.loadFromPreview = loadFromPreview;
    }

    public String getCreatedConsolationTN() {
        return createdConsolationTN;
    }

    public void setCreatedConsolationTN(String createdConsolationTN) {
        this.createdConsolationTN = createdConsolationTN;
    }

    //endregion

}